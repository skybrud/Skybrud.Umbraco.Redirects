import { LitElement, html, css } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { UmbPropertyValueChangeEvent } from "@umbraco-cms/backoffice/property-editor";

import "@skybrud-redirects/elements/destination";

export default class SkybrudOutboundPropertyEditorElement extends UmbElementMixin(LitElement) {

    static properties = {
        value: { type: Object }
    };

    constructor() {
        super();
        this.value = undefined;
    }

    connectedCallback() {
        super.connectedCallback();
        if (this.value && !this.value.destination) {
            this.value = undefined;
            this.#dispatchChange();
        }
    }

    #setValue(nextValue) {
        this.value = nextValue || undefined;
        this.#dispatchChange();
    }

    #dispatchChange() {
        this.dispatchEvent(new UmbPropertyValueChangeEvent());
    }

    #updateRedirectType(event) {
        if (!this.value) return;
        this.#setValue({ ...this.value, permanent: event.target.value === 'true' });
    }

    #updateForward(event) {
        if (!this.value) return;
        this.#setValue({ ...this.value, forward: event.target.value === 'true' });
    }

    #onDestinationChange(ev) {

        if (!ev.detail?.value) {
            this.#setValue(null);
            return;
        }

        this.#setValue({
            permanent: this.value?.permanent === true,
            forward: this.value?.forward === true,
            destination: ev.detail.value
        });

    }

    render() {

        const permanent = this.value?.permanent === true;
        const forward = this.value?.forward === true;
        const destination = this.value?.destination;

        if (!destination) {
            return html`
                <redirects-destination @change="${(e) => this.#onDestinationChange(e)}"></redirects-destination>
            `;
        }

        return html`
            <div class="box">
                <div class="field">
                    <label>${this.localize.term("redirectsProperties_redirectType")}</label>
                    <div class="description">${this.localize.term("redirectsProperties_redirectTypeDescription")}</div>
                    <div class="field-value radio-group">
                        <uui-radio-group .value=${String(permanent)} @change=${this.#updateRedirectType}>
                            <uui-radio value="true" label="${this.localize.term("redirects_permanent")}"></uui-radio>
                            <uui-radio value="false" label="${this.localize.term("redirects_temporary")}"></uui-radio>
                        </uui-radio-group>
                    </div>
                </div>
                <div class="field">
                    <label>${this.localize.term("redirectsProperties_forwardQueryString")}</label>
                    <div class="description">${this.localize.term("redirectsProperties_forwardQueryStringDescription")}</div>
                    <div class="field-value radio-group">
                        <uui-radio-group .value=${String(forward)} @change=${this.#updateForward}>
                            <uui-radio value="true" label="${this.localize.term("redirects_enabled")}"></uui-radio>
                            <uui-radio value="false" label="${this.localize.term("redirects_disabled")}"></uui-radio>
                        </uui-radio-group>
                    </div>
                </div>
                <div class="field">
                    <label>${this.localize.term("redirectsProperties_destination")}</label>
                    <div class="description">${this.localize.term("redirectsProperties_destinationDescription")}</div>
                    <div class="field-value">
                        <redirects-destination .value=${this.value?.destination} @change="${(e) => this.#onDestinationChange(e)}"></redirects-destination>
                    </div>
                </div>
            </box>
        `;

    }

    static styles = css`

        uui-radio-group {
            display: flex;
            gap: 20px;
        }

        .box {
            border-radius: var(--uui-border-radius, 3px);
            border: 1px solid var(--uui-color-border);
            padding: 15px;
        }

        .field + .field {
            margin-top: var(--uui-size-layout-1);
        }

        label {
            display: block;
            font-weight: 700;
            margin-bottom: var(--uui-size-2);
        }

        .description {
            color: var(--uui-color-text-alt);
            margin-bottom: var(--uui-size-3);
        }

    `;

}

customElements.define("skybrud-outbound-property-editor", SkybrudOutboundPropertyEditorElement);