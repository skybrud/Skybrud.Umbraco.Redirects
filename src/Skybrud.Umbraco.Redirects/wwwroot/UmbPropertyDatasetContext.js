import { UmbContextBase } from '@umbraco-cms/backoffice/class-api';
import { UmbContextToken } from "@umbraco-cms/backoffice/context-api";
import { UmbVariantId } from "@umbraco-cms/backoffice/variant";

export const FAKE_UMB_PROPERTY_DATASET_CONTEXT = new UmbContextToken('UmbPropertyDatasetContext');

export class FakeUmbPropertyDatasetContext extends UmbContextBase {

    getVariantId() {
        return UmbVariantId.CreateInvariant();
    }

}