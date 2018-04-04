import { Component, Input, NgModule, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';

import { ScBizFxProperty } from '@sitecore/bizfx';

@Component({
    selector: 'sc-bizfx-actionproperty-tags',
    styleUrls: ['./sc-bizfx-actionproperty-tags.component.scss'],
    template: `
    <div class="form-group">
      <label for="tag-{{property.Name}}">{{property.DisplayName}}</label>
      <tag-input
        id="tag-{{property.Name}}"
        [(ngModel)]='items'
        (submit)='$event.stopPropagation()'
        theme='sitecore-theme'
        (onBlur)="onInputBlurred()">
      </tag-input>
    </div>`
})

export class ScBizFxActionPropertyTagsComponent implements OnInit {
    @Input() actionForm: FormGroup;
    @Input() property: ScBizFxProperty;

    items = [];
    ngOnInit(): void {
        if (this.property.Value !== null && this.property.Value !== undefined) {
            const tags = JSON.parse(this.property.Value);
            tags.forEach(tag => {
                const tagModel = {
                    display: tag,
                    value: tag,
                    readonly: false
                };
                this.items.push(tagModel);
            });
        }
    }

    onInputBlurred(): void {
        const current = [];
        this.items.forEach(item => {
            current.push(item.value);
        });

        this.actionForm.controls[this.property.Name].setValue(JSON.stringify(current));
    }
}
