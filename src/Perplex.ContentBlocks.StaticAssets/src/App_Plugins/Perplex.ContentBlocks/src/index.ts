import variables from './css/variables.css?inline';
export * from "./perplex-content-blocks";
export * from "./components/block/pcb-block.ts";
export * from "./pcb-preview.ts";


// Components:
export * from './components/block'
export * from './components/modals'
export * from './components/block/blockDefinition/block-definition.ts'
export * from './components/block/inlineLayoutSwitch/pcb-inline-layout-switch.ts'



const styleTag = document.createElement('style');
styleTag.textContent = variables;
document.head.appendChild(styleTag);
