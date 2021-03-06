﻿module Foundation.View.Directives {

    @Foundation.Core.DirectiveDependency({ name: 'dtoForm' })
    export class DefaultDtoFormDirective implements Foundation.ViewModel.Contracts.IDirective {
        public getDirectiveFactory(): angular.IDirectiveFactory {
            return () => ({
                scope: false,
                transclude: true,
                terminal: true,
                replace: true,
                restrict: 'E',
                template: (element: JQuery, attrs: angular.IAttributes) => {
                    let defaultNgModelOptions = `ng-model-options="{ updateOn : 'default blur' , allowInvalid : true , debounce: { 'default': 250, 'blur': 0 } }"`;
                    if (attrs['ngModelOptions'] != null)
                        defaultNgModelOptions = '';
                    return `<ng-form ${defaultNgModelOptions} dto-form-service ng-transclude></ng-form>`;
                }
            });
        }
    }
}