import 'rxjs/add/operator/switchMap';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';

import { ScBizFxViewsService, ScBizFxContextService } from '@sitecore/bizfx';
import { ScBizFxView, ScBizFxAction } from '@sitecore/bizfx';

@Component({
  selector: 'sc-bizfx-view',
  templateUrl: './sc-bizfx-view.component.html'
})

export class ScBizFxViewComponent implements OnInit {
  view: ScBizFxView;
  action: ScBizFxAction;
  loading = true;

  constructor(
    private viewsService: ScBizFxViewsService,
    private bizFxContext: ScBizFxContextService,
    private route: ActivatedRoute) {
  }

  ngOnInit(): void {
    this.viewsService.announcePageType(this.route.snapshot.data.pageType);

    this.route.params
      .switchMap((params: Params) => this.viewsService.getView(params['viewName'], params['entityId'], '', params['itemId']))
      .subscribe(view => {
        this.loading = false;
        this.view = view;
      });


    this.bizFxContext.environment$.subscribe(environment => this.getView());

    this.bizFxContext.language$.subscribe(language => this.getView());

    this.viewsService.actionAnnounced$.subscribe(
      action => {
        this.action = action;
        setTimeout(() => this.action = null, 2000);
        this.getView();
      });
  }

  getView() {
    this.loading = true;

    this.viewsService.getView(
      this.view ? this.view.Name : '',
      this.view && this.view.EntityId ? this.view.EntityId : '',
      this.view && this.view.Action ? this.view.Action : '',
      this.view && this.view.ItemId ? this.view.ItemId : '')
      .then(view => {
        this.view = view;
        this.loading = false;
      });
  }
}
