import { Component, Input, OnInit, ChangeDetectorRef } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/debounceTime';

import { SortDirection } from '@speak/ng-bcl/core/types';
import { SortHeaderState } from '@speak/ng-bcl/table';

import { ScBizFxSearchService } from '@sitecore/bizfx';
import { ScBizFxView, ScBizFxProperty, ScBizFxActionMessage } from '@sitecore/bizfx';

@Component({
  selector: 'sc-bizfx-searchview',
  templateUrl: './sc-bizfx-searchview.component.html'
})

export class ScBizFxSearchViewComponent implements OnInit {
  @Input() view: ScBizFxView;

  selectedResult: ScBizFxView;
  results: Observable<ScBizFxView>;
  resultsView: ScBizFxView;
  searchTerms = new Subject<string>();
  searching: boolean;
  term: ScBizFxProperty;
  orderBy: ScBizFxProperty;
  filter: ScBizFxProperty;
  skip: ScBizFxProperty;
  top: ScBizFxProperty;
  count = 0;
  pageSize = 10;
  sortState = [];
  messages: ScBizFxActionMessage[];

  propView: ScBizFxProperty[];

  constructor(
    private searchService: ScBizFxSearchService,
    private cdr: ChangeDetectorRef) {
  }

  ngOnInit(): void {
    this.term = this.view.Properties.filter(p => p.Name === 'Term')[0];
    this.filter = this.view.Properties.filter(p => p.Name === 'Filter')[0];
    this.orderBy = this.view.Properties.filter(p => p.Name === 'OrderBy')[0];
    this.skip = this.view.Properties.filter(p => p.Name === 'Skip')[0];
    this.top = this.view.Properties.filter(p => p.Name === 'Top')[0];

    this.results = this.searchTerms
      .debounceTime(1000)
      .switchMap(term => term
        ? this.searchService.search(this.view)
        : Observable.of<ScBizFxView>())
      .catch(error => {
        this.searching = false;
        return Observable.of<ScBizFxView>();
      });

    this.results.subscribe(view => {
      this.resultsView = view;

      if (view.ChildViews.length > 0) {
        this.selectedResult = view.ChildViews[0];
        this.resultsView.ItemId = this.selectedResult.ItemId;
        this.propView = view && this.selectedResult ? this.selectedResult.Properties.filter(prop => !prop.IsHidden) : [];
      }

      const count = view.Properties.filter(p => p.Name === 'Count')[0];
      this.count = count !== null && count !== undefined ? +view.Properties.filter(p => p.Name === 'Count')[0].Value : 0;
      this.searching = false;
      this.cdr.detectChanges();
    });

    this.searchService.actionInfosAnnounced$.subscribe(warnings => this.messages = warnings);
  }

  search(sort: any, page?: number): void {
    if (this.term.Value === '') {
      return;
    }

    if (sort) {
      this.orderBy.Value = Array.isArray(sort) && sort.length ? `${sort[0].id} ${sort[0].direction}` : '';
    }

    if (page !== undefined && page !== null) {
      const pageIndex = page - 1;
      this.top.Value = this.pageSize.toString();
      this.skip.Value = (pageIndex * this.pageSize).toString();
    }

    this.searching = true;
    this.searchTerms.next(this.term.Value);
    this.messages = [];
  }

  onSelect(id: string): void {
    const view = this.resultsView.ChildViews.find(v => v.ItemId === id);

    this.selectedResult = view;
    this.resultsView.ItemId = view ? view.ItemId : null;
  }

  onSortChange(sortState: SortHeaderState[]) {
    this.sortState = sortState;
    this.search(sortState);
  }

  getDirection(id: string): SortDirection {
    const state = this.sortState.find((s: SortHeaderState) => s.id === id);
    return state ? state.direction : '';
  }
}
