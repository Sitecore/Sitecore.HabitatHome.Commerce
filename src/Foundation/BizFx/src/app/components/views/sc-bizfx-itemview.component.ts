import { Component, Input } from '@angular/core';

import { ScBizFxProperty, ScBizFxView } from '@sitecore/bizfx';

@Component({
  selector: 'sc-bizfx-itemview',
  templateUrl: './sc-bizfx-itemview.component.html',
  styleUrls: ['./sc-bizfx-itemview.component.css']
})

export class ScBizFxItemViewComponent {
  selected: boolean;

  @Input() view: ScBizFxView;
  @Input() property: ScBizFxProperty;

  constructor() {
    this.selected = false;
  }
}
