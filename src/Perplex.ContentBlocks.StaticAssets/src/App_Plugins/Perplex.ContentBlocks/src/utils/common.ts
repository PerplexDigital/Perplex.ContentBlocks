import {UmbId} from "@umbraco-cms/backoffice/id";

export const createUdi = (entityType: string) => {
    return `umb://${entityType}/${UmbId.new().replace(/-/g, "")}`;
}