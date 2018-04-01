import { Component, Input, OnInit } from '@angular/core';
import { ScBizFxProperty } from '@sitecore/bizfx';

@Component({
    selector: 'sc-bizfx-viewproperty-tags',
    template: `<div *ngFor="let tag of tags">{{tag}}</div>`
})

export class ScBizFxViewPropertyTagsComponent implements OnInit {
    @Input() property: ScBizFxProperty;

    tags: any[];
    ngOnInit(): void {
        if (this.property.Value !== null && this.property.Value !== undefined) {
            this.tags = JSON.parse(this.property.Value);
        }
    }
}
