import { Component, Input, OnInit } from '@angular/core';
import { FormGroup, FormArray, FormBuilder, FormControl, Validators } from '@angular/forms';

import { ScBizFxContextService, ScBizFxProperty, ScBizFxView, getPropertyValidators, deepClone } from '@sitecore/bizfx';

@Component({
    selector: 'sc-bizfx-actiongrid',
    templateUrl: './sc-bizfx-actiongrid.component.html'
})

export class ScBizFxActionGridComponent implements OnInit {
    @Input() view: ScBizFxView;
    @Input() actionForm: FormGroup;
    @Input() grid: FormArray;
    allowAdd = true;

    headers: ScBizFxProperty[] = [];
    children: ScBizFxView[] = [];

    constructor(
        public bizFxContext: ScBizFxContextService,
        private fb: FormBuilder) {
    }

    ngOnInit(): void {
        if (this.view === undefined || !this.view.ChildViews || this.view.ChildViews.length === 0) { return; }

        this.headers = this.view.ChildViews[0].Properties.filter(p => !p.IsHidden);
        this.children = this.view.ChildViews;

        const propertyAllowAdd = this.view.Properties.filter(p => p.Name.toLowerCase() === 'allowadd')[0];
        if (propertyAllowAdd !== undefined && propertyAllowAdd.Value === 'false') {
            this.allowAdd = false;
        }
    }

    propertyControl(groupIndex: number, property: ScBizFxProperty) {
        return this.grid.controls[groupIndex].get(property.Name);
    }

    isValid(groupIndex: number, property: ScBizFxProperty) {
        const control = this.propertyControl(groupIndex, property);

        if (property !== undefined && property.UiType === 'Autocomplete') {
            const inputElement = (<HTMLInputElement>document.getElementsByClassName('ng-autocomplete-input')[0]);
            if (inputElement !== undefined && inputElement.value.length > 0) {
                control.setValue(inputElement.value);
                control.markAsTouched();
            }
        }

        return control.valid || control.pristine;
    }


    addRow(): void {
        const clone = deepClone(this.view.ChildViews[0]);
        this.children.push(clone);

        const cloneGroup: any = {};
        clone.Properties.forEach(property => {
            const validators = getPropertyValidators(property, this.bizFxContext.language);
            cloneGroup[property.Name] = new FormControl({ value: property.Value || null, disabled: property.IsReadOnly }, validators);
        });
        this.grid.push(this.fb.group(cloneGroup));
    }

    removeRow(index: number): void {
        if (index <= -1) { return; }

        this.children.splice(index, 1);
        this.grid.removeAt(index);
    }
}
