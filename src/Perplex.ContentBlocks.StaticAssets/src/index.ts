import variables from './css/variables.css?inline';
export * from './perplex-content-blocks';
export * from './pcb-preview.ts';

// Components:
export * from './components/block';
export * from './components/modals';
export * from './components/block/blockDefinition/block-definition.ts';
export * from './components/block/inlineLayoutSwitch/pcb-inline-layout-switch.ts';
export * from './components/block/pcb-block.ts';
export * from './components/block/blockSpacer/pcb-block-spacer.ts';
export * from './components/dragAndDrop/pcb-drag-and-drop.ts';
export * from './components/dragAndDrop/pcb-drag-item.ts';

const styleTag = document.createElement('style');
styleTag.textContent = variables;
document.head.appendChild(styleTag);
