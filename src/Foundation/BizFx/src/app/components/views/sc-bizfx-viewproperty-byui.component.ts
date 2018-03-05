import { Component, Input, OnInit } from '@angular/core';

import { Angular2Csv } from 'angular2-csv/Angular2-csv';

import { ScBizFxProperty, ScBizFxView } from '@sitecore/bizfx';

@Component({
  selector: 'sc-bizfx-viewproperty-byui',
  templateUrl: './sc-bizfx-viewproperty-byui.component.html'
})

export class ScBizFxViewPropertyByUiComponent implements OnInit {
  @Input() property: ScBizFxProperty;
  @Input() view: ScBizFxView;
  @Input() hideHeader: boolean;

  list: any[];

  ngOnInit(): void {
    if (this.property.UiType === 'List'
      && this.property.Value !== null
      && this.property.Value !== undefined) {
      this.list = JSON.parse(this.property.Value);
    }
  }

  downloadCsv() {
    return new Angular2Csv(JSON.parse(this.property.Value), `${this.property.Name}`);
  }

  buildSubItemLink(): string {
    const parts = this.view.ItemId.split('|');
    if (parts.length === 2) {
      return `${parts[0]}/${parts[1]}`;
    }

    console.warn('Invalid sub-item link format. Expected: view.Item = \'entityId|subItemId\'');

    // Use regular item link format as a fallback
    return `${this.view.EntityId}/${this.view.ItemId}`;
  }
}
