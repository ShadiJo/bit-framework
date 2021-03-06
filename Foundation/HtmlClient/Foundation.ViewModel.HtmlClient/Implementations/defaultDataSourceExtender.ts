﻿/// <reference path="../../foundation.core.htmlclient/foundation.core.d.ts" />
module Foundation.ViewModel.Implementations {
    export class DefaultDataSourceExtender implements Core.Contracts.IAppEvents {
        @Core.Log()
        public async onAppStartup(): Promise<void> {

            kendo.data.DataSource.prototype.dataView = function () {
                return (this as kendo.data.DataSource)
                    .view()
                    .map(vi => {
                        let viItem = (vi as any);
                        return viItem.innerInstance != null ? viItem.innerInstance() : viItem;
                    });
            };

            kendo.data.DataSource.prototype.onCurrentChanged = function (action) {

                let dataSource = this;

                dataSource.onCurrentChangedHandlers = dataSource.onCurrentChangedHandlers || [];

                if (action != null) {
                    dataSource.onCurrentChangedHandlers.push(action);
                } else {

                    for (let handler of dataSource.onCurrentChangedHandlers) {
                        handler();
                    }
                }

            };

            kendo.data.DataSource.prototype.asChildOf = function (parentDataSource, childKeys, parentKeys) {

                let childDataSource: kendo.data.DataSource = this;

                parentDataSource.onCurrentChanged(async () => {

                    if (childDataSource.current != null)
                        childDataSource.current = null;

                    let parentKeyCurrentValues: any[] = null;
                    let currentParent = parentDataSource.current;

                    await new Promise<void>((resolve) => {
                        window.setTimeout(() => resolve(), 350);
                    });

                    if (currentParent != parentDataSource.current)
                        return;

                    if (currentParent == null || parentKeys.some(pk => currentParent[pk] == null)) {
                        childDataSource.fetch();
                        return;
                    }
                    else {
                        parentKeyCurrentValues = parentKeys.map(pk => { return currentParent[pk]; });;
                    }

                    let parentChildFilters: kendo.data.DataSourceFilters = {
                        logic: "and", filters: childKeys.map((ck, ckI) => {
                            return { field: ck, value: parentKeyCurrentValues[ckI], operator: 'eq', isParentChildFilter: true, parentField: parentKeys[ckI] }
                        })
                    };

                    let currentChildFilters = childDataSource.filter();

                    if (currentChildFilters == null || ((currentChildFilters instanceof Array) && currentChildFilters.length == 0) || (currentChildFilters.filters != null && (currentChildFilters.filters instanceof Array) && currentChildFilters.filters.length == 0)) {
                        childDataSource.filter(parentChildFilters);
                    }
                    else {
                        let checkFilters = (filtersToBeChecked) => {
                            for (let flt of filtersToBeChecked as any) {
                                let childKeyI = childKeys.findIndex(ck => ck == flt.field);
                                if (childKeyI != -1) {
                                    flt.value = parentKeyCurrentValues[childKeyI];
                                    flt.isParentChildFilter = true;
                                }
                            }
                        };
                        let filters = currentChildFilters.filters as any;
                        checkFilters(filters);
                        if (filters != null) {
                            filters.filter(innerFilters => innerFilters.filters != null).forEach(innerFilters => checkFilters(innerFilters.filters));
                        }
                        childDataSource.filter(currentChildFilters);
                    }
                });

                let originalChildTransportRead = childDataSource['transport'].read;

                childDataSource['transport'].read = function (options) {

                    let currentParent = parentDataSource.current;

                    let parentChildFilterIsValid = options.data.filter != null && options.data.filter.filters != null;

                    let checkFilters = (filtersToBeChecked) => {
                        for (let flt of filtersToBeChecked as any) {
                            if (flt.isParentChildFilter == true && (currentParent != null && flt.value != currentParent[flt.parentField]))
                                parentChildFilterIsValid = false;
                        }
                    };

                    if (parentChildFilterIsValid == true) {
                        let filters = options.data.filter.filters;
                        checkFilters(filters);
                        if (filters != null) {
                            filters.filter(innerFilters => innerFilters.filters != null).forEach(innerFilters => checkFilters(innerFilters.filters));
                        }
                    }

                    if (currentParent == null || parentKeys.some(pk => currentParent[pk] == null) || parentChildFilterIsValid == false) {
                        options.success({ data: [], length: 0 });
                    }
                    else {
                        return originalChildTransportRead.apply(this, arguments);
                    }
                }

                let originalChildTransportCreate = childDataSource['transport'].create;

                childDataSource['transport'].create = function (options, models): void {

                    let currentParent = parentDataSource.current;

                    if (currentParent == null || parentKeys.some(pk => currentParent[pk] == null)) {
                        throw new Error("Parent data source's current item is null or new");
                    }

                    for (let model of models) {
                        for (let i = 0; i < childKeys.length; i++) {
                            model[childKeys[i]] = currentParent[parentKeys[i]];
                            (model.innerInstance != null ? model.innerInstance() : model)[childKeys[i]] = currentParent[parentKeys[i]];
                        }
                    }

                    return originalChildTransportCreate.apply(this, arguments);
                }
            }
        }
    }
}