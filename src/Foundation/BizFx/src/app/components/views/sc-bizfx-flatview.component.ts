import { Component, Input } from '@angular/core';

import { ScBizFxProperty, ScBizFxView, ScBizFxContextService } from '@sitecore/bizfx';

@Component({
  selector: 'sc-bizfx-flatview',
  templateUrl: './sc-bizfx-flatview.component.html',
  styleUrls: ['./sc-bizfx-flatview.component.css']
})

export class ScBizFxFlatViewComponent {
  @Input() view: ScBizFxView;

  constructor(
    public bizFxContext: ScBizFxContextService) {
  }

  getList(property: ScBizFxProperty) {
    return property.Value != null ? JSON.parse(property.Value) : [];
  }
}
