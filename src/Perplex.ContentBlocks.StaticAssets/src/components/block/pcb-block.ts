import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import {
    classMap,
    css,
    customElement,
    html,
    property,
    repeat,
    state,
    unsafeCSS,
    nothing,
    PropertyValues,
} from '@umbraco-cms/backoffice/external/lit';
import type { UmbPropertyTypeContainerModel, UmbPropertyTypeModel } from '@umbraco-cms/backoffice/content-type';
import { PerplexContentBlocksPropertyDatasetContext } from '../../editor/perplex-content-blocks-dataset-context.ts';
import { UmbDataTypeDetailModel, UmbDataTypeDetailRepository } from '@umbraco-cms/backoffice/data-type';
import { UmbDocumentTypeDetailModel, UmbDocumentTypeDetailRepository } from '@umbraco-cms/backoffice/document-type';
import { UMB_VALIDATION_CONTEXT, UmbValidationController } from '@umbraco-cms/backoffice/validation';
import contentBlockName from '../../utils/contentBlockName.ts';
import { Group, PerplexBlockDefinition, PerplexContentBlocksBlock, Section, Tab } from '../../types.ts';
import { BlockUpdatedEvent, ON_BLOCK_LAYOUT_CHANGE, ON_BLOCK_REMOVE } from '../../events/block.ts';
import { connect } from 'pwa-helpers';

import baseStyles from './../../css/base.css?inline';
import { AppState, store } from '../../state/store.ts';
import { PcbDragAndDrop } from '../dragAndDrop/pcb-drag-and-drop.ts';
import { propertyAliasPrefix } from '../../utils/block.ts';
import { withEditorId } from '../../events/generic.ts';
import { consume } from '@lit/context';
import { editorContext } from '../../context';

@customElement('pcb-block')
export default class PerplexContentBlocksBlockElement extends connect(store)(UmbLitElement) {
    @property({ type: Boolean, reflect: true })
    dragging: boolean | null = null;

    @property({ attribute: false })
    section: Section = Section.CONTENT;

    @property({ attribute: false })
    index!: number;

    @property({
        type: Boolean,
        reflect: true,
        attribute: 'draggable',
        converter: {
            toAttribute: (value: boolean) => (value ? 'true' : 'false'),
            fromAttribute: (value: string | null) => value === 'true',
        },
    })
    draggable: boolean = false;

    @state()
    invalid: boolean = false;

    updated(changedProps: PropertyValues) {
        super.updated(changedProps);

        if (changedProps.has('draggable')) {
            if (this.draggable) {
                this.addEventListener('dragstart', this.onDragStart);
                this.addEventListener('dragend', this.onDragEnd);
            } else {
                this.removeEventListener('dragstart', this.onDragStart);
                this.removeEventListener('dragend', this.onDragEnd);
            }
        }

        // Ensure vertical layout for umb-property
        this.updateComplete.then(() => {
            Array.from(this.renderRoot.querySelectorAll('umb-property')).forEach((umbProp: any) => {
                const layout = umbProp?.shadowRoot?.querySelector('umb-property-layout');
                if (layout) layout.orientation = 'vertical';
            });
        });
    }

    @property({ attribute: false })
    collapsed: boolean = true;

    @property()
    definition?: PerplexBlockDefinition;

    @property({ attribute: false })
    block!: PerplexContentBlocksBlock;

    @property({ attribute: false })
    removeBlock!: (udi: string) => void;

    @property({ attribute: false })
    dataPath!: string;

    @property({ attribute: false })
    openModal!: (section: Section, insertAtIndex: number) => any;

    @state()
    private ok: boolean = false;

    @state()
    properties!: UmbPropertyTypeModel[];

    @state()
    isMandatory: boolean = false;

    @state()
    removing: boolean = false;

    @state()
    isDraggingBlock: boolean = false;

    #contentTypeRepository = new UmbDocumentTypeDetailRepository(this);

    #dataTypeRepository = new UmbDataTypeDetailRepository(this);

    #dataTypes: {
        [key: string]: UmbDataTypeDetailModel;
    } = {};

    #validationController?: UmbValidationController;

    @consume({ context: editorContext })
    editorId!: string;

    connectedCallback() {
        super.connectedCallback();
        const errors: string[] = [];

        if (!this.block) {
            errors.push('block property is required');
        }

        if (!this.removeBlock) {
            errors.push('removeBlock property is required');
        }

        if (!this.removeBlock) {
            errors.push('onChange property is required');
        }

        if (errors.length > 0) {
            throw new Error(errors.join(' | '));
        }

        this.addEventListener(ON_BLOCK_LAYOUT_CHANGE, (e: Event) => this.onLayoutChange(e as CustomEvent));
    }

    disconnectedCallback() {
        super.disconnectedCallback();
        this.removeEventListener(ON_BLOCK_REMOVE, this.onBlockRemoveClick);
        this.clearValidationMessages();
    }

    onDragStart = (event: DragEvent) => {
        this.dragging = true;
        this.isDraggingBlock = true;
        const rect = this.getBoundingClientRect();
        PcbDragAndDrop.activeDrag = { element: this, height: rect.height };
        event.dataTransfer!.effectAllowed = 'move';
        event.dataTransfer!.setData('text/plain', '');
    };

    onDragEnd = () => {
        const active = PcbDragAndDrop.activeDrag;
        if (active) {
            active.element.style.display = ''; // restore visibility
        }
        PcbDragAndDrop.activeDrag = null;
        this.dragging = null;
        this.isDraggingBlock = false;
    };

    stateChanged(state: AppState) {
        this.isDraggingBlock = state.ui.isDraggingBlock;
        if (state.presets.value && state.presets.value.blocks && state.presets.value.blocks.length > 0) {
            const presetItem = state.presets.value.blocks.find((item) => {
                return item.id === this.block.presetId && item.definitionId === this.block.definitionId;
            });
            this.isMandatory = presetItem ? presetItem.isMandatory : false;
        }

        if (state.presets.value.header && this.section === Section.HEADER) {
            const presetItem = state.presets.value.header;
            this.isMandatory =
                presetItem.id === this.block.presetId && presetItem.definitionId === this.block.definitionId
                    ? presetItem.isMandatory
                    : false;
        }
    }

    onLayoutChange = (event: CustomEvent<any>) => {
        // do nothing if the layout didn't change
        if (this.block.layoutId === event.detail.selectedLayout.id) {
            return;
        }

        const updatedBlock: PerplexContentBlocksBlock = {
            ...this.block,
            layoutId: event.detail.selectedLayout.id,
        };

        if (this.definition) {
            this.dispatchEvent(
                withEditorId(BlockUpdatedEvent(updatedBlock, this.definition, this.section), this.editorId),
            );
        }
    };

    constructor() {
        super();
        this.consumeContext(UMB_VALIDATION_CONTEXT, (ctx) => {
            this.#validationController = ctx;

            ctx?.messages.messages.subscribe((messages) => {
                this.invalid = false;

                for (const message of messages) {
                    if (message.path.indexOf(this.block.id) !== -1) {
                        this.invalid = true;
                    }
                }
            });
        });

        this.addEventListener(ON_BLOCK_REMOVE, this.onBlockRemoveClick);
    }

    onBlockRemoveClick = () => {
        this.removing = true;

        // Remove the block after the animation has finished
        setTimeout(() => {
            this.removeBlock(this.block.id);
        }, 250);
    };

    onBlockUpdate = (block: PerplexContentBlocksBlock) => {
        if (this.definition) {
            this.dispatchEvent(withEditorId(BlockUpdatedEvent(block, this.definition, this.section), this.editorId));
        }
    };

    async firstUpdated() {
        const elementTypeResponse = await this.#contentTypeRepository.requestByUnique(
            this.block.content.contentTypeKey,
        );

        if (elementTypeResponse.data == null) {
            throw new Error(`Cannot retrieve content type ${this.block.content.contentTypeKey}`);
        }

        const elementType = elementTypeResponse.data;
        this.properties = await this.#getOrderedProperties(elementType);

        new PerplexContentBlocksPropertyDatasetContext(this, this.block, this.onBlockUpdate);

        const dataTypeUniques = new Set(this.properties.map((p) => p.dataType.unique));

        for (const dataTypeUnique of dataTypeUniques) {
            const dataTypeResponse = await this.#dataTypeRepository.requestByUnique(dataTypeUnique);
            if (dataTypeResponse.data == null) {
                throw new Error(`Cannot retrieve data type ${dataTypeUnique}`);
            }

            this.#dataTypes[dataTypeUnique] = dataTypeResponse.data;
        }

        this.ok = true;
    }

    async #getOrderedProperties(elementType: UmbDocumentTypeDetailModel) {
        if (elementType.compositions.length === 0) {
            return [...elementType.properties];
        }

        // When the element type has compositions we need to ensure the properties are returned in the proper order,
        // we cannot simply concat all contentType.properties together unfortunately.
        // The order is determined by the tabs and groups defined on each content type.
        // The order is:
        // 1) For each group without a parent tab:
        //      1) Properties in that group
        // 2) For each tab:
        //      1) Properties directly on the tab (not in a group)
        //      2) For each group in that tab:
        //          1) Properties in that group
        // Ensure each step orders by sortOrder of the tab, group or property.
        // Note that we must first merge tabs and groups at each level based on their name,
        // e.g. if an Element Type defines 'Tab A' and a composition also defines 'Tab A' we need to merge them.
        // Same goes for groups, also those within merged tabs.

        const contentTypes: UmbDocumentTypeDetailModel[] = [];
        contentTypes.push(elementType);

        for (const composition of elementType.compositions) {
            const compositionTypeResponse = await this.#contentTypeRepository.requestByUnique(
                composition.contentType.unique,
            );

            if (compositionTypeResponse.data == null) {
                // Ignore
                continue;
            }

            contentTypes.push(compositionTypeResponse.data);
        }

        const tabs: Tab[] = [];
        const groups: Group[] = [];

        const propertiesByContainerId: { [key: string]: UmbPropertyTypeModel[] } = {};

        for (const contentType of contentTypes) {
            for (const property of contentType.properties) {
                if (property.container?.id == null) continue;

                if (!propertiesByContainerId[property.container.id])
                    propertiesByContainerId[property.container.id] = [];

                propertiesByContainerId[property.container.id].push(property);
            }

            const containersByParentId: { [key: string]: UmbPropertyTypeContainerModel[] } = {};
            for (const container of contentType.containers) {
                const parentId = container.parent?.id;
                if (parentId == null) continue;
                if (!containersByParentId[parentId]) containersByParentId[parentId] = [];
                containersByParentId[parentId].push(container);
            }

            for (const container of contentType.containers) {
                if (container.type === 'Tab') {
                    const groups: Group[] = (containersByParentId[container.id] || []).map(buildGroup);

                    const tab: Tab = {
                        id: container.id,
                        name: container.name,
                        sortOrder: container.sortOrder,
                        groups: groups,
                        properties: propertiesByContainerId[container.id] || [],
                    };

                    const existingTab = tabs.find((t) => t.name === tab.name);
                    if (existingTab) {
                        // Merge
                        const merged = mergeTabs(existingTab, tab);
                        tabs.splice(tabs.indexOf(existingTab), 1, merged);
                    } else {
                        tabs.push(tab);
                    }
                } else if (container.type === 'Group' && container.parent?.id == null) {
                    const group = buildGroup(container);
                    const existingGroup = groups.find((g) => g.name === group.name);
                    if (existingGroup) {
                        // Merge
                        const merged = mergeGroups(existingGroup, group);
                        groups.splice(groups.indexOf(existingGroup), 1, merged);
                    } else {
                        groups.push(group);
                    }
                }
            }

            function buildGroup(container: UmbPropertyTypeContainerModel): Group {
                return {
                    id: container.id,
                    name: container.name,
                    sortOrder: container.sortOrder,
                    properties: propertiesByContainerId[container.id] || [],
                };
            }
        }

        tabs.sort((a, b) => a.sortOrder - b.sortOrder);
        groups.sort((a, b) => a.sortOrder - b.sortOrder);

        const properties: UmbPropertyTypeModel[] = [];

        for (const group of groups) {
            addSortedProperties(group);
        }

        for (const tab of tabs) {
            addSortedProperties(tab);

            for (const group of tab.groups) {
                addSortedProperties(group);
            }
        }

        function addSortedProperties(container: Group | Tab) {
            const props = container.properties.slice();
            props.sort((a, b) => a.sortOrder - b.sortOrder);
            properties.push(...props);
        }

        function mergeTabs(tabA: Tab, tabB: Tab): Tab {
            const properties = [...tabA.properties, ...tabB.properties];
            properties.sort((a, b) => a.sortOrder - b.sortOrder);

            const groups: Group[] = [];
            for (const group of [...tabA.groups, ...tabB.groups]) {
                const existingGroup = groups.find((g) => g.name === group.name);
                if (existingGroup) {
                    const merged = mergeGroups(existingGroup, group);
                    groups.splice(groups.indexOf(existingGroup), 1, merged);
                } else {
                    groups.push(group);
                }
            }

            const mergedTab: Tab = {
                id: tabA.id,
                name: tabA.name,
                sortOrder: Math.min(tabA.sortOrder, tabB.sortOrder),
                properties,
                groups,
            };

            return mergedTab;
        }

        function mergeGroups(groupA: Group, groupB: Group): Group {
            const properties = [...groupA.properties, ...groupB.properties];
            properties.sort((a, b) => a.sortOrder - b.sortOrder);

            const mergedGroup: Group = {
                id: groupA.id,
                name: groupA.name,
                sortOrder: Math.min(groupA.sortOrder, groupB.sortOrder),
                properties,
            };

            return mergedGroup;
        }

        return properties;
    }

    clearValidationMessages() {
        for (const property of this.properties) {
            const path = `${this.dataPath}.${this.block.id}.${property.alias}`;
            this.#validationController?.messages.removeMessagesByTypeAndPath('client', path);
            this.#validationController?.messages.removeMessagesByTypeAndPath('server', path);
        }
    }

    render() {
        if (!this.ok || this.definition == null) {
            return nothing;
        }

        const classes = {
            block: true,
            block__removing: this.removing,
            block__invalid: this.invalid && !this.removing,
        };

        return html` ${this.section !== Section.HEADER
                ? html`<pcb-block-spacer
                      .openModal=${this.openModal}
                      .index=${this.index}
                  ></pcb-block-spacer>`
                : nothing}
            <div class="${classMap(classes)}">
                <pcb-block-head
                    .block=${this.block}
                    .id=${this.block.id}
                    .blockDefinitionName=${this.definition?.name}
                    .collapsed="${this.collapsed}"
                    .blockTemplateName="${contentBlockName(this.definition?.blockNameTemplate ?? '', this.block)}"
                    .definition=${this.definition}
                    .section=${this.section}
                    .isDraggingBlock=${this.isDraggingBlock}
                    .isMandatory=${this.isMandatory}
                >
                </pcb-block-head>
                <div
                    class="
                    block__body
                    ${classMap({
                        'block__body--open': !this.collapsed,
                        'block__body--hidden': this.collapsed,
                        'block__body--dragging': this.isDraggingBlock && this.collapsed,
                    })}"
                >
                    <div>
                        ${repeat(
                            this.properties,
                            (property) => property.unique,
                            (property) => {
                                const dataType = this.#dataTypes[property.dataType.unique];
                                if (dataType == null) throw new Error('missing data type');

                                return html` <umb-property
                                    .dataPath=${`${this.dataPath}.${this.block.id}.${property.alias}`}
                                    .alias=${propertyAliasPrefix(this.block) + property.alias}
                                    .label=${property.name}
                                    .description=${property.description}
                                    .appearance=${property.appearance}
                                    property-editor-ui-alias=${dataType.editorUiAlias}
                                    orientation="vertical"
                                    .config=${dataType.values}
                                    .validation=${property.validation}
                                >
                                </umb-property>`;
                            },
                        )}
                    </div>
                </div>
            </div>`;
    }

    static styles = [
        unsafeCSS(baseStyles),
        css`
            :host {
                width: 100%;
                overflow: hidden;
            }

            .block {
                box-shadow: var(--bs-base);

                &.block__removing {
                    opacity: 0;
                    transform: scaleY(0);
                    transition:
                        opacity 250ms,
                        transform 250ms;
                }

                &.block__invalid {
                    border: 2px solid var(--uui-color-danger);
                }

                .block__body {
                    background-color: var(--c-mystic);
                    display: grid;

                    transition:
                        250ms grid-template-rows ease,
                        250ms padding ease;

                    &.block__body--hidden {
                        grid-template-rows: 0fr;
                        padding: 0 calc(var(--s) * 8);
                    }

                    &.block__body--open {
                        grid-template-rows: 1fr;
                        padding: calc(var(--s) * 6) calc(var(--s) * 8);
                    }

                    &.block__body--dragging {
                        display: none;
                    }

                    > div {
                        overflow: hidden;
                    }
                }
            }
        `,
    ];
}
