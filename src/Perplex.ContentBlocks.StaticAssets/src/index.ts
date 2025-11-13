import './components';
import './editor';
import './pcb-preview';

import variables from './css/variables.css?inline';
const styleTag = document.createElement('style');
styleTag.textContent = variables;
document.head.appendChild(styleTag);
