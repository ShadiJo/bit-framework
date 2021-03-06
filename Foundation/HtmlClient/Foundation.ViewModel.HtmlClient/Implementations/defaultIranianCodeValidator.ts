﻿/// <reference path="../../foundation.core.htmlclient/foundation.core.d.ts" />
module Foundation.ViewModel.Implementations {
    export class DefaultIranianCodeValidator implements Core.Contracts.IIranianCodeValidator {
        @Core.Log()
        public nationalCodeIsValid(code: string): boolean {

            if (code == null)
                return false;

            if (!/^\d{10}$/.test(code))
                return false;

            let check = parseInt(code[9]);

            let sum = [0, 1, 2, 3, 4, 5, 6, 7, 8]
                .map((x) => { return parseInt(code[x]) * (10 - x); })
                .reduce((x, y) => { return x + y; }) % 11;

            return sum < 2 && check == sum || sum >= 2 && check + sum == 11;

        }

        @Core.Log()
        public companyCodeIsValid(companyCode: string): boolean {

            let num = 0;

            let result = (/^\d{11}$/).test(companyCode);

            if (result) {

                let invalidCompanyCodes = ['00000000000', '11111111111', '22222222222', '33333333333', '44444444444',
                    '55555555555', '66666666666', '77777777777', '88888888888', '99999999999'];

                result = invalidCompanyCodes.indexOf(companyCode) === -1;

                if (result) {
                    let c = parseInt(companyCode[10]);
                    let c10 = parseInt(companyCode[9]);

                    let n = (parseInt(companyCode[0]) + c10 + 2) * 29 +
                        (parseInt(companyCode[1]) + c10 + 2) * 27 +
                        (parseInt(companyCode[2]) + c10 + 2) * 23 +
                        (parseInt(companyCode[3]) + c10 + 2) * 19 +
                        (parseInt(companyCode[4]) + c10 + 2) * 17 +
                        (parseInt(companyCode[5]) + c10 + 2) * 29 +
                        (parseInt(companyCode[6]) + c10 + 2) * 27 +
                        (parseInt(companyCode[7]) + c10 + 2) * 23 +
                        (parseInt(companyCode[8]) + c10 + 2) * 19 +
                        (parseInt(companyCode[9]) + c10 + 2) * 17;

                    let r = n - (Math.floor((n / 11)) * 11);

                    if (r === 10)
                        r = 0

                    if (r === c)
                        result = true;
                    else
                        result = false;
                }

            }

            return result;
        }
    }
}