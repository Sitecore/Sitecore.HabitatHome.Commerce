import { Component, Input } from '@angular/core';

import { ScBizFxView } from '@sitecore/bizfx';

@Component({
  selector: 'sc-bizfx-listpolicies',
  templateUrl: './sc-bizfx-listPolicies.component.html'
})

export class ScBizFxListPoliciesComponent {
  @Input() view: ScBizFxView;
  @Input() policies: ScBizFxView[];
}
