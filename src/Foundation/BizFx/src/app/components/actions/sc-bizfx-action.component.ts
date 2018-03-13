import 'rxjs/add/operator/switchMap';
import { Component, OnInit, Inject, Output, EventEmitter } from '@angular/core';
import { FormArray, FormGroup, FormControl, FormBuilder, AbstractControl } from '@angular/forms';
import { Subscription } from 'rxjs/Subscription';

import { NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';

import { ScDialogService } from '@speak/ng-bcl/dialog';

import { ScBizFxContextService, ScBizFxViewsService, getLocaleNumberSymbol, NumberSymbol, parseDecimalNumber } from '@sitecore/bizfx';
import { ScBizFxView, ScBizFxProperty, ScBizFxAction, ScBizFxActionMessage } from '@sitecore/bizfx';
import { getPropertyValidators } from '@sitecore/bizfx';

@Component({
  selector: 'sc-bizfx-action',
  templateUrl: './sc-bizfx-action.component.html',
  styleUrls: ['./sc-bizfx-action.component.css']
})

export class ScBizFxActionComponent implements OnInit {
  @Output() submitted: EventEmitter<null> = new EventEmitter();

  view: ScBizFxView;
  actionForm: FormGroup;
  action: ScBizFxAction;
  data: ScBizFxView;
  messages: ScBizFxActionMessage[];
  loadingView = false;

  constructor(
    public bizFxContext: ScBizFxContextService,
    private viewsService: ScBizFxViewsService,
    private dialogService: ScDialogService,
    private fb: FormBuilder) {
    this.actionForm = new FormGroup({});
  }

  ngOnInit(): void {
    if (this.data.Actions[0] !== null && this.data.Actions[0] !== undefined) {
      this.action = this.data.Actions[0];
    }

    this.getActionView();

    this.viewsService.actionErrorsAnnounced$.subscribe(errors => this.messages = errors);
  }

  get grid(): FormArray {
    return this.actionForm.get('Grid') as FormArray;
  }

  submitAction(): void {
    if (!this.action.RequiresConfirmation) {
      this.prepareSaveView();
      this.doAction();
    } else {
      this.doAction();
    }
  }

  cancelAction(): void {
    this.dialogService.close();
  }

  protected getActionView() {
    this.loadingView = true;
    this.viewsService.getView(this.data.Name, this.data.EntityId, this.data.Action, this.data.ItemId)
      .then(view => {
        this.view = view;
        this.buildForm();
        this.loadingView = false;

        if (!this.action) {
          this.action = new ScBizFxAction(view.Action, view.EntityId, view.Action);
        }
      });
  }

  protected doAction() {
    this.viewsService
      .doAction(this.view)
      .then(actionResult => {
        this.view.ActionResult = actionResult;

        if (actionResult.NextView) {
          this.view = actionResult.NextView;
          this.buildForm();

          if (this.view.Properties.filter(p => p.IsHidden).length === this.view.Properties.length
            && this.view.ChildViews.length === 0) {
            this.submitAction();
          }
        } else if (actionResult.ResponseCode === 'Ok') {
          this.submitted.emit();
          this.dialogService.close();
        }
      });
  }

  protected buildForm() {
    if (this.view.Name === '' || this.view.Name === 'null' || this.view.Name === undefined) { return; }

    const group = this.buildGroup(this.view.Properties);

    if (this.view.UiHint === 'Grid') {
      const childrenGroups = new FormArray([]);
      this.view.ChildViews.forEach((child, index) => childrenGroups.push(new FormGroup(this.buildGroup(child.Properties))));
      group['Grid'] = childrenGroups;
    }

    this.actionForm = new FormGroup(group);
  }

  protected buildGroup(properties: ScBizFxProperty[]): any {
    const group: any = {};

    properties.forEach(property => {
      const validators = getPropertyValidators(property, this.bizFxContext.language);
      const control = new FormControl({ value: property.Value || null, disabled: property.IsReadOnly }, validators);
      this.transformProperty(property, control);
      group[property.Name] = control;
    });

    return group;
  }

  protected prepareSaveView() {
    this.view.Properties.map(property => {
      this.mapProperty(property, this.actionForm.get(property.Name));
    });

    if (this.view.UiHint === 'Grid') {
      this.view.ChildViews.forEach((child, index) => {
        const control = this.grid.at(index);
        if (control) { child.Properties.map(property => this.mapProperty(property, control.get(property.Name))); }
      });
    }
  }

  protected mapProperty(property: ScBizFxProperty, control: AbstractControl): any {
    if (!control) {
      return;
    }

    switch (property.OriginalType) {
      case 'System.DateTimeOffset':
        if (Date.parse(control.value)) {
          property.Value = (control.value as Date).toISOString();
        }

        break;
      case 'System.String':
        if (control.value != null) {
          property.Value = control.value.trim();
        }

        break;
      case 'System.Decimal':
        const currencyDecimal = getLocaleNumberSymbol(this.bizFxContext.language, NumberSymbol.CurrencyDecimal);
        const currencyGroup = getLocaleNumberSymbol(this.bizFxContext.language, NumberSymbol.CurrencyGroup);
        const options = [currencyGroup, currencyDecimal];
        property.Value = parseDecimalNumber(control.value, options);

        break;
      default:
        if (control.value != null) {
          property.Value = control.value;
        }

        break;
    }
  }

  protected transformProperty(property: ScBizFxProperty, control: FormControl) {
    switch (property.OriginalType) {
      case 'System.DateTimeOffset':
        const date = new Date(property.Value);
        control.setValue(date);

        break;
      case 'System.Boolean':
        let checked = false;
        if (property.Value !== undefined && property.Value !== null) {
          checked = property.Value.toLowerCase() === 'true';
        }
        control.setValue(checked);

        break;
      default:
        break;
    }
  }
}
