import { Component, Input, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';

import { ScBizFxSearchService } from '@sitecore/bizfx';
import { ScBizFxProperty, ScBizFxView } from '@sitecore/bizfx';

@Component({
  selector: 'sc-bizfx-mediapicker',
  templateUrl: './sc-bizfx-mediapicker.component.html'
})

export class ScBizFxMediaPickerComponent implements OnInit {
  @Input() view: ScBizFxView;
  @Input() actionForm: FormGroup;

  selectedView: ScBizFxView;
  top: ScBizFxProperty;
  private skip: ScBizFxProperty;
  count = 0;
  pageSize: number;
  resultsView: ScBizFxView[];
  private searchView: ScBizFxView;

  constructor(
    private searchService: ScBizFxSearchService) {
  }

  ngOnInit(): void {
    if (this.view === undefined || !this.view.ChildViews || this.view.ChildViews.length === 0) {
      return;
    }

    this.setData(this.view);
  }

  search(page: number): void {
    this.top.Value = this.pageSize.toString();
    this.skip.Value = (page - 1).toString();

    this.searchService.search(this.searchView).then(view => this.setData(view));
  }

  onSelect(id: string): void {
    const view = this.resultsView.find(v => v.ItemId === id);

    this.selectedView = view;
    this.view.ItemId = view.ItemId;
  }

  protected setData(view: ScBizFxView) {
    this.resultsView = view.ChildViews.filter(v => v.UiHint !== 'Search');

    if (this.resultsView && this.resultsView[0]) {
      this.selectedView = this.resultsView[0];
      this.view.ItemId = this.resultsView[0].ItemId;
    }

    this.searchView = view.ChildViews.filter(v => v.UiHint === 'Search')[0];
    if (this.searchView) {
      this.top = this.searchView.Properties.filter(p => p.Name === 'PageSize')[0];
      this.skip = this.searchView.Properties.filter(p => p.Name === 'Page')[0];
      this.pageSize = +this.top.Value;
      this.count = this.searchView.Properties.filter(p => p.Name === 'Count')[0]
        ? +this.searchView.Properties.filter(p => p.Name === 'Count')[0].Value
        : 0;
    }
  }
}
