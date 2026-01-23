export class PcbFocusBlockInPreviewEvent extends Event {
	public static readonly TYPE = 'PcbFocusBlockInPreviewEvent';
	public blockId: string;

	public constructor(blockId: string) {
		super(PcbFocusBlockInPreviewEvent.TYPE, { bubbles: true, composed: true, cancelable: true });
		this.blockId = blockId;
	}
}
