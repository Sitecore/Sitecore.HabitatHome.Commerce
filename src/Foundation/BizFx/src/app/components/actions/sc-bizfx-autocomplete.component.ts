import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { Response } from '@angular/http';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Params } from '@angular/router';
import { AfterViewInit } from '@angular/core/src/metadata/lifecycle_hooks';
import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';
import { FormControl } from '@angular/forms';

import 'rxjs/add/observable/of';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/debounceTime';
import 'rxjs/add/operator/distinctUntilChanged';
import 'rxjs/add/observable/fromEvent';

import { CreateNewAutocompleteGroup, SelectedAutocompleteItem, NgAutocompleteComponent, AutocompleteGroup } from 'ng-auto-complete';

import { ScBizFxView } from '@sitecore/bizfx';
import {
    ScBizFxContextService,
    ScBizFxBaseService,
    ScBizFxAuthService
} from '@sitecore/bizfx';

@Component({
    selector: 'sc-bizfx-autocomplete',
    template: `<ng-autocomplete
    (selected)="Selected($event)"
    [classes]="['']" [group]="group"
    (keyup)="onKey($event)"
    ng-model="searchField">
</ng-autocomplete>`,
    styles: ['::ng-deep .ng-dropdown.open{background-color: white ;border: black ;z-index:1;' +
        'border-style: solid ;border-width: 1px ;box-shadow: 3px 3px 10px 4px rgba(0,0,0,0.25)' +
        ';color: black ;position: fixed ;max-height: 150px ;overflow: scroll }' +
        '::ng-deep .ng-autocomplete-placeholder{display:none}']
})

export class ScBizFxAutocompleteComponent extends ScBizFxBaseService implements AfterViewInit {
    @Input() view: ScBizFxView;
    @Input() property;
    term: string;
    results: Observable<ScBizFxView>;
    resultsView: ScBizFxView;
    searchTerms = new Subject<string>();
    searchControl: FormControl;
    searching: boolean;
    count: Number;
    data: any;
    searchValue: string;
    target: any;
    lastTimestamp: any;
    debounceFunction: any;
    PolicyScope: any;
    haveSearched: boolean;
    _completer: any;
    doSearch: boolean;
    @ViewChild(NgAutocompleteComponent) public completer: NgAutocompleteComponent;
    public group = [CreateNewAutocompleteGroup('', 'completer', [], { titleKey: 'title', childrenKey: null })];

    constructor(
        protected http: HttpClient,
        protected bizFxContext: ScBizFxContextService,
        protected authService: ScBizFxAuthService) {
        super(http, bizFxContext, authService);
    }

    ngAfterViewInit(): void {
        this.doSearch = false;
        this.haveSearched = false;
        this.lastTimestamp = -1000000;
        // we need to apply the form styling to this object.
        if (<HTMLInputElement>document.getElementsByClassName('ng-autocomplete-input')[0]) {
            (<HTMLInputElement>document.getElementsByClassName('ng-autocomplete-input')[0]).className += ' form-control';
        }
    }

    RunSearch(event: any): void {
        this.searchValue = event.target.value;
        this.target = event.target;
        if (this.searchValue.length > 3 && this.doSearch === true) {
            this.doSearch = false;
            let SearchPolicy = '';
            this.PolicyScope = '';
            // we need to get the search scope
            const Policies = <any>this.property;
            for (let i = 0; i < Policies.length; i++) {
                if (Policies[i]['@odata.type'] !== undefined &&
                    Policies[i]['@odata.type'] === '#Sitecore.Commerce.Plugin.Search.SearchScopePolicy') {
                    SearchPolicy = Policies[i].Name;
                }
                if (Policies[i].PolicyId === 'EntityType') {
                    this.PolicyScope = Policies[i].Models[0].Name;
                }
            }
            const termFormat = this.searchValue + '*';
            const body = {
                'scope': SearchPolicy,
                'term': termFormat,
                'filter': '',
                'orderBy': '',
                'skip': 0,
                'top': 100
            };
            const headers = this.authService.getHeadersWithAuth();
            const results = '';
            this.http.put(this.bizFxContext.doSearch(), body, { headers: headers,
                withCredentials: true }).map(res => res).subscribe(ret => {
                this.data = ret;
                setTimeout(this.UpdateItems(), 1000);
            });
        }
    }

    onKey(event: any) {
        this.searchValue = event.target.value;
        if (event.keyCode === 8 && this.searchValue.length === 0) {
            this.haveSearched = false;
        }

        if (this.searchValue.length === 4 && this.haveSearched === false || event.keyCode === 13) {
            this.haveSearched = true;
            this.doSearch = true;
            this.RunSearch(event);
            this.lastTimestamp = event.timeStamp;
        } else {
            this.haveSearched = false;
        }
}

    InputCleared(event: any) {
    }

    UpdateItems() {
        let searchResults;
        if (this.data !== undefined && this.data['Models'] !== undefined) {
            searchResults = this.data['Models'][0]['ChildViews'];
            const searchValues = [];
            const Policies = <any>this.property;
            let variantSearch = false;
            for (let i = 0; i < Policies.length; i++) {
                if (Policies[i].PolicyId === 'EntityType') {
                    this.PolicyScope = Policies[i].Models[0].Name;
                    if (Policies[i].Models[1] !== undefined && Policies[i].Models[1].Name === 'SearchVariants') {
                        variantSearch = true;
                    }
                }
            }
            for (const entry of searchResults) {
                // probably going to want to make this a little more robust
                const entityId = (<string>entry['EntityId']);
                const match = 'Entity-' + this.PolicyScope;
                if (entityId.startsWith(match, 0) || this.PolicyScope === '') {
                    const properties = entry['Properties'];
                    for (const prop of properties) {
                        if (prop['DisplayName'] === 'Display Name') {
                            searchValues.push({ 'title': prop['Value'] + ' | ' + entry['EntityId'], 'id': entry['EntityId'] });
                            const len = entry['Properties'].length;
                            let variantDisplayNames = -1;
                            let variantIds = -1;
                            let hasVariants = false;
                            if (variantSearch) {
                                for (let i = 0; i < len; i++) {
                                    if (variantDisplayNames !== -1 && variantIds !== -1) {
                                        break;
                                    }
                                    if (entry['Properties'][i] !== undefined && entry['Properties'][i].Name === 'variantdisplayname') {
                                        variantDisplayNames = i;
                                        hasVariants = true;
                                    }
                                    if (entry['Properties'][i] !== undefined && entry['Properties'][i].Name === 'variantid') {
                                        variantIds = i;
                                        hasVariants = true;
                                    }
                                }
                                // if we have variants, push those results.
                                if (hasVariants) {
                                    const variantDisplayNameArray = (String)(entry['Properties'][variantDisplayNames].Value).split('|');
                                    const variantIdsArray = (String)(entry['Properties'][variantIds].Value).split('|');
                                    for (let i = 0; i < variantDisplayNameArray.length; i++) {
                                        searchValues.push({
                                            'title': '-' + variantDisplayNameArray[i]
                                                + ' (' + variantIdsArray[i] + ')' + ' | '
                                                + entry['EntityId'], 'id': entry['EntityId'] + '|' + variantIdsArray[i]
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            const component = this.group[0];
            component.SetValues(searchValues);
            const scopedCompleter = this.completer;
            setTimeout(function () {
                const inputElement = (<HTMLInputElement>document.getElementsByClassName('ng-autocomplete-input')[0]);
                inputElement.className += ' form-control'; // The autocomplete likes to reset styling.
                const ele = (<any>document.getElementsByClassName('ng-dropdown open'))[0];
                ele.style.visibility = 'show';
                scopedCompleter.TriggerChange();
            }, 1);
        }
    }

    /**
     *
     * @param item
     * @constructor
     */
    Selected(item: SelectedAutocompleteItem) {
        setTimeout(function () {
            const inputElement = (<HTMLInputElement>document.getElementsByClassName('ng-autocomplete-input')[0]);
            if (item !== undefined && item !== null
                && item.item !== undefined && item.item !== null
                && item.item.id !== undefined && item.item.id !== null) {
                inputElement.value = <string>item.item.id;
            }
        }, 1);
    }
}
