import { Component, Input, OnInit } from '@angular/core';

import { ScBizFxProperty, ScBizFxView, ScBizFxContextService } from '@sitecore/bizfx';

@Component({
  selector: 'sc-bizfx-viewproperty-bytype',
  styleUrls: ['./sc-bizfx-viewproperty-bytype.component.scss'],
  templateUrl: './sc-bizfx-viewproperty-bytype.component.html'
})

export class ScBizFxViewPropertyByTypeComponent implements OnInit {
  @Input() property: ScBizFxProperty;
  @Input() view: ScBizFxView;
  @Input() hideHeader: boolean;

  list: any[];

  constructor(
    private bizFxContext: ScBizFxContextService) {
  }

  ngOnInit(): void {
    if (this.property.OriginalType === 'List'
      && this.property.Value !== null
      && this.property.Value !== undefined) {
      this.list = JSON.parse(this.property.Value);
    }
  }
}
