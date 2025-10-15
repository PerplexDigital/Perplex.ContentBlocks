const gulp = require("gulp");
const embedTemplates = require("gulp-angular-embed-templates");
const concat = require("gulp-concat");
const minify = require("gulp-minify");
const fs = require("fs").promises;
const glob = require("glob-promise");
const path = require("path");
const less = require("gulp-less");
const insert = require("gulp-insert");
const cleancss = require("gulp-clean-css");

gulp.task("css:build", compileLess);
gulp.task("js:build", compileJs);
gulp.task("watch", watch);
gulp.task("bundle", bundle);

const appPluginsSubDir = "Perplex.ContentBlocks";
const srcDir = path.join("App_Plugins", appPluginsSubDir);
const lessFileName = "perplex.content-blocks.less";
const jsOutputFileName = "perplex.content-blocks.js";
const outputDir = "../wwwroot";

// We need to distribute this file as is since it cannot be embedded into the JS.
// In order to do so we need to turn it into an AngularJS component.
const staticFilesToKeep = "perplex.content-blocks.html";

async function bundle() {
    await compileLess();
    await compileJs();
}

function watch() {
    gulp.watch(toGlobPath(path.join(srcDir, "**", "*.{html,js,less}")), bundle);
}

async function compileLess() {
    const lessFile = path.join(srcDir, lessFileName);
    const destination = "."; // Same as input less file
    return new Promise((resolve) => {
        gulp
            .src(lessFile)
            .pipe(less())
            .pipe(cleancss())
            .pipe(gulp.dest(destination))
            .on("end", resolve);
    });
}

async function compileJs() {
    const destination = path.join(outputDir, "App_Plugins", appPluginsSubDir);

    // Bundle all HTML + JS source files into 1 file for the output
    const outputFileName = jsOutputFileName;

    await preBuild(outputDir, srcDir, destination);
    await build(destination, outputFileName);
    await postBuild(destination, outputFileName);
};

async function preBuild(outputDir, source, destination) {
    // Clean output
    await fs.rm(outputDir, { recursive: true, force: true });

    // Copy App_Plugins/Perplex.ContentBlocks to output directory
    await fs.cp(source, destination, { recursive: true });

    // *-view.html files will instead be replaced by AngularJS interceptors that provide
    // a base64 data URI so the physical files HTML can be removed.
    // The interceptor files will be bundled into the output JS file.
    await transformHtmlViewsToInterceptors(destination);
}

async function transformHtmlViewsToInterceptors(src) {
    const htmlViews = await globFiles(src, "/**/*-view.html");

    for (const htmlView of htmlViews) {
        const fileName = path.basename(htmlView);
        const fileContent = await fs.readFile(htmlView, "utf8");
        const interceptorFileName = htmlView.replace(/\.html$/, ".js");
        const base64 = Buffer.from(fileContent).toString("base64");

        const interceptorContent = `
angular.module("umbraco").config(["$httpProvider", function ($httpProvider) {
    const re = /${fileName.replace(/\./g, "\\.")}$/i;

    $httpProvider.interceptors.push(function () {
        return {
            request: function (config) {
                if (re.test(config.url)) {
                    config.url = "data:text/html;charset=utf-8;base64,${base64}";
                }

                return config;
            }
        }
    });
}]);`;

        await fs.writeFile(interceptorFileName, interceptorContent);
    }
}

async function build(destination, outputFileName) {
    // We only specify JS files but the embedTemplates plugin will then
    // also bundle the referenced HTML files
    const filesGlob = [
        // perplex.requires.js NEEDS to be the first file in the bundle as it defines
        // our AngularJS module which is used by other JS files.
        path.join(destination, "perplex.content-blocks.requires.js"),
        path.join(destination, "**", "!(perplex.content-blocks.requires)*.js"),
    ];

    await bundleFiles(filesGlob, outputFileName, destination);
}

async function postBuild(destination, outputFileName) {
    // Remove all static files that will be bundled in the build step (all less/html/js files).
    const staticFiles = await globFiles(destination, "/**/*.{js,html,less}");

    // Do not remove the bundled output file
    const filesToRemove = staticFiles
        .filter(file => path.basename(file) !== outputFileName)
        .filter(file => !staticFilesToKeep.includes(path.basename(file)))

    await Promise.all(filesToRemove.map(fs.unlink));

    // Remove any empty directories remaining in the output
    await removeEmptyDirectories(destination);
}

async function globFiles(directory, pattern) {
    const expr = toGlobPath(directory) + pattern;
    return await glob(expr, { nodir: true });
}

async function removeEmptyDirectories(sourceDir) {
    const dir = await fs.opendir(sourceDir);
    for await (const entry of dir) {
        if(entry.isFile()) {
            continue;
        }

        const fullPath = path.join(sourceDir, entry.name);

        // Remove any nested empty directories first
        await removeEmptyDirectories(fullPath);

        // Then check if we are now empty
        const subDir = await fs.opendir(fullPath);
        const isEmpty = await isDirectoryEmpty(subDir);

        if(isEmpty) {
            await fs.rm(fullPath, { recursive: true, force: true });
        }
    }
}

async function isDirectoryEmpty(directory) {
    for await (const entry of directory) {
        // If directory contains any file or directory: not empty
        return false;
    }

    // If we reach here: empty
    return true;
}


function bundleFiles(src, jsFile, dest) {
    const basePath = ".";

    return new Promise((resolve, _) => {
        gulp.src(src)
            .pipe(embedTemplates({ basePath, minimize: { empty: true } }))
            .pipe(concat(jsFile))
            .pipe(minify({
                ext: { min: ".js" },
                noSource: true,
                mangle: false,
                compress: {
                    keep_classnames: true,
                    keep_fnames: true
                }
            }))
            .pipe(insert.wrap("(function(){","})();"))
            .pipe(gulp.dest(dest))
            .on("end", resolve);
    });
}

function toGlobPath(path) {
    // glob requires the path separator to be "/", it cannot handle "\" for some reason.
    // So if the given path contains any "\" we will replace those by "/".
    return path.replace(/\\/g, "/");
}
