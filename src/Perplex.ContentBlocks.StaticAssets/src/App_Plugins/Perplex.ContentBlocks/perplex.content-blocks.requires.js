// Register with Angular
angular.module("perplexContentBlocks", ["slick"]);

// Register as dependency for Umbraco
angular.module("umbraco").requires.push("perplexContentBlocks");
