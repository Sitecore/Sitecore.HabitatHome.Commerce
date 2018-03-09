import { Component, Input, OnInit, OnChanges, SimpleChanges } from '@angular/core';

import { ScBizFxView, ScBizFxProperty } from '@sitecore/bizfx';
import { ScBizFxViewsService } from '@sitecore/bizfx';

@Component({
  selector: 'sc-bizfx-tableview',
  templateUrl: './sc-bizfx-tableview.component.html'
})

export class ScBizFxTableViewComponent implements OnInit, OnChanges {
  @Input() view: ScBizFxView;

  selectedView: ScBizFxView;
  top: ScBizFxProperty;
  private skip: ScBizFxProperty;
  count = 0;
  pageSize: number;
  paginating = false;
  showPagination: boolean;
  page = 1;

  constructor(
    private viewsService: ScBizFxViewsService) {
  }

  ngOnInit(): void {
    if (this.view === undefined || !this.view.ChildViews || this.view.ChildViews.length === 0) {
      return;
    }

    this.setData();
  }

  ngOnChanges(changes: SimpleChanges): void {
    this.page = 1;
    this.skip = this.view.Properties.filter(p => p.Name === 'Skip')[0];
  }

  paginate(page: number): void {
    if (!this.view) {
      return;
    }

    this.page = page;
    this.top.Value = this.pageSize.toString();
    this.skip.Value = ((page - 1) * this.pageSize).toString();
    this.paginating = true;
    this.view.ChildViews = [];

    this.viewsService
      .doAction(this.view)
      .then(actionResult => {
        this.view.ActionResult = actionResult;
        this.view = actionResult.NextView;

        this.setData();
      });
  }

  onSelect(id: string): void {
    const view = this.view.ChildViews.find(c => c.ItemId === id);

    if (view) {
      this.selectedView = view;
      this.view.ItemId = view.ItemId;
    }
  }

  protected setData() {
    this.paginating = false;

    if (!this.view) {
      return;
    }

    this.selectedView = this.view.ChildViews[0];
    this.view.ItemId = this.view.ChildViews[0].ItemId;

    this.top = this.view.Properties.filter(p => p.Name === 'Top')[0];
    this.skip = this.view.Properties.filter(p => p.Name === 'Skip')[0];
    const count = this.view.Properties.filter(p => p.Name === 'Count')[0];
    this.showPagination = this.skip && this.top && count && this.view && this.view.ChildViews.length > 0;
    this.pageSize = this.showPagination ? +this.top.Value : 0;
    this.count = this.showPagination && count ? +count.Value : 0;
  }
}
