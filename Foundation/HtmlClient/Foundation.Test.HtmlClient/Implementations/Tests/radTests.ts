﻿module Foundation.Test.Implementations.Tests {
    export class RadTests {

        public static async testRadGridFormViewModelAdd(): Promise<void> {

            const uiAutomation = new UIAutomation<ViewModels.RadGridFormViewModel>(angular.element("#radGridView"));

            let vm = uiAutomation.formViewModel;

            let grid = uiAutomation.view.find("#gridView").data('kendoGrid');

            await uiAutomation.delay();

            await grid.addRow();

            let parentEntity = vm.parentEntitiesDataSource.current as Model.DomainModels.ParentEntity;

            parentEntity.Name = "!";

            grid.saveRow();

            await uiAutomation.delay();

            if (parentEntity.Id != '999') {
                throw new Error("rad grid add problem");
            }

            if (parentEntity.Name != "!") {
                throw new Error("rad grid add problem");
            }

            if ($("[name='Id']").filter((tdIndex: number, tdElement: HTMLTableRowElement) => tdElement.innerText == '999').length == 0) {
                throw new Error("rad grid add problem");
            }
        }

        public static async testRadComboFormViewModel(): Promise<void> {

            const uiAutomation = new UIAutomation<ViewModels.RadComboFormViewModel>(angular.element("#radComboView"));

            uiAutomation.formViewModel.testModelsDataSource.current = uiAutomation.formViewModel.testModelsDataSource.dataView<Foundation.Test.Model.DomainModels.TestModel>().find(i => i['StringProperty'] == "String2");

            let vm = uiAutomation.formViewModel;

            uiAutomation.updateUI();

            if (vm.model.TestModel.Id != '2')
                throw new Error("rad combo problem");

            if (uiAutomation.view.find("#test2").text() != "2")
                throw new Error("rad combo problem");

            if (uiAutomation.view.find("#test3").text() != "String2")
                throw new Error("rad combo problem");

            await uiAutomation.formViewModel.setCurrent();

            if (vm.model.TestModel.Id != '1')
                throw new Error("rad combo problem");

            if (uiAutomation.view.find("#test2").text() != "1")
                throw new Error("rad combo problem");

            if (uiAutomation.view.find("#test3").text() != "String1")
                throw new Error("rad combo problem");
        }
    }
}