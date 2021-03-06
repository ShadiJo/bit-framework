﻿/// <reference path="../../foundation.core.htmlclient/declarations.d.ts" />
module Foundation.Test.Implementations {

    export class UIAutomation<TFormViewModel> {

        public view: JQuery;

        constructor(view: JQuery) {

            if (view == null)
                throw new Error('view is null');

            this.view = view;
        }

        public get formViewModel(): TFormViewModel {
            return this.getFormViewModel(this.view);
        }

        protected get $scope(): ng.IScope {
            return this.getBindingContext<ng.IScope>(this.view);
        }

        public delay(time: number = 250): Promise<void> {

            return new Promise<void>((resolve) => {

                setTimeout(() => {

                    resolve();

                }, time);

            });

        }

        public async waitForPredicate(predicate: () => boolean | Promise<boolean>, time: number = 10000): Promise<boolean> {

            if (predicate == null)
                throw new Error('predicate is null');

            for (let i = 0; i < 10; i++) {
                let result = await predicate();
                if (result == true)
                    return true;
                await this.delay(time / 10);
            }

            return false;
        }

        public getFormViewModel(element: JQuery): TFormViewModel {

            if (element == null)
                throw new Error('element is null');

            return this.getBindingContext<TFormViewModel>(element, "vm");

        }

        public getBindingContext<TBindingContext>(element: JQuery, name?: string): TBindingContext {

            if (element == null)
                throw new Error('element is null');

            const $scope: any = angular.element(element[0]).scope();

            if (name != null)
                return $scope[name] as TBindingContext;
            else
                return $scope as TBindingContext;
        }

        public getForm(element: JQuery): any {
            if (element == null)
                throw new Error('element is null');
            return this.getBindingContext(element, element.attr('name'));
        }

        public updateUI(): void {
            Foundation.ViewModel.ScopeManager.update$scope(this.$scope);
        }
    }
}