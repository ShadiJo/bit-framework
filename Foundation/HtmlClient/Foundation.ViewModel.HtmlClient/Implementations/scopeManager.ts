﻿module Foundation.ViewModel {
    export class ScopeManager {

        public static Use$ApplyAsync = true;

        public static update$scope = ($scope: ng.IScope): void => {

            if (ScopeManager.Use$ApplyAsync == true)
                $scope.$applyAsync();
            else if ($scope.$$phase != "$apply" && $scope.$$phase != "$digest" && $scope.$root.$$phase != "$apply" && $scope.$root.$$phase != "$digest")
                $scope.$apply();

        }

    }
}