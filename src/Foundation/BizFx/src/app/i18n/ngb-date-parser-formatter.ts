import { Injectable } from '@angular/core';
import { NgbDateParserFormatter, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';

import { ScBizFxContextService } from '@sitecore/bizfx';

import {formatDate} from './format-date';

function isNumber(value: any): boolean {
    return !isNaN(toInteger(value));
}

function toInteger(value: any): number {
    return parseInt(`${value}`, 10);
}

@Injectable()
export class NgbDateScParserFormatter extends NgbDateParserFormatter {

    constructor(
        private bizFxContext: ScBizFxContextService) {
        super();
    }

    parse(value: string): NgbDateStruct {
        if (value) {
            const language = this.bizFxContext.language;
            const rawDateTime = new Date(value.replace(/:([^:]*)$/,  '$1'));
            const date = formatDate(rawDateTime, 'shortDate', language, '');

            const dateParts = value.trim().split('/');
            if (dateParts.length === 1 && isNumber(dateParts[0])) {
                return { year: toInteger(dateParts[0]), month: null, day: null };
            } else if (dateParts.length === 2 && isNumber(dateParts[0]) && isNumber(dateParts[1])) {
                return { year: toInteger(dateParts[1]), month: toInteger(dateParts[0]), day: null };
            } else if (dateParts.length === 3 && isNumber(dateParts[0]) && isNumber(dateParts[1]) && isNumber(dateParts[2])) {
                return { year: toInteger(dateParts[2]), month: toInteger(dateParts[1]), day: toInteger(dateParts[0]) };
            }
        }
        return null;
    }

    format(date: NgbDateStruct): string {
        let stringDate = '';
        if (date) {
            const language = this.bizFxContext.language;
            const rawDateTime = new Date(date.year, date.month - 1, date.day);
            stringDate = formatDate(rawDateTime, 'shortDate', language, '');
        }
        return stringDate;
    }
}
