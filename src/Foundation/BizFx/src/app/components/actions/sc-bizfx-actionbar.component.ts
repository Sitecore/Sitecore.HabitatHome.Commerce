import { Component, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { Location } from '@angular/common';

import { ScDialogService } from '@speak/ng-bcl/dialog';
import { ScAction } from '@speak/ng-bcl/action-control';

import { ScBizFxAction, ScBizFxView } from '@sitecore/bizfx';
import { ScBizFxActionComponent } from './sc-bizfx-action.component';
import { ScBizFxContextService, ScBizFxViewsService } from '@sitecore/bizfx';

@Component({
  selector: 'sc-bizfx-actionbar',
  templateUrl: './sc-bizfx-actionbar.component.html'
})

export class ScBizFxActionBarComponent {
  @Input() view: ScBizFxView;

  constructor(
    private location: Location,
    private router: Router,
    public dialogService: ScDialogService,
    private viewsService: ScBizFxViewsService) {
  }

  doAction(action: ScBizFxAction) {
    if (action.UiHint === 'RelatedList') {
      this.selectAction(action);
    } else {
      this.openActionDialog(action);
    }
  }

  protected openActionDialog(action: ScBizFxAction) {
    const view = new ScBizFxView(
      this.view.EntityId !== null && this.view.EntityId !== undefined && this.view.EntityId !== 'undefined' ? this.view.EntityId : '',
      this.view.ItemId !== null && this.view.ItemId !== undefined && this.view.ItemId !== 'undefined' ? this.view.ItemId : '');

    if (action.IsMultiStep) {
      view.Name = action.FirstStepAction.EntityView;
      view.Action = action.FirstStepAction.Name;
    } else {
      view.Name = action.EntityView;
      view.Action = action.Name;
    }

    view.Actions = [];
    view.Actions.push(action);

    this.dialogService.open(ScBizFxActionComponent);
    const dialog: ScBizFxActionComponent = this.dialogService.contentComponentRef.instance;
    dialog.data = view;
    dialog.submitted.subscribe(() => {
      this.viewsService.announceAction(action);
    });
  }

  protected selectAction(action: ScBizFxAction): void {
    if (action.UiHint === 'RelatedList') {
      this.router.navigate(['/entityView', action.EntityView]);
    } else {
      if (action.IsMultiStep) {
        this.router.navigate([
          '/action',
          action.FirstStepAction.EntityView,
          this.view.EntityId,
          action.FirstStepAction.Name,
          this.view.ItemId ? this.view.ItemId : '']);
      } else {
        this.router.navigate([
          '/action',
          action.EntityView,
          this.view.EntityId,
          action.Name,
          this.view.ItemId ? this.view.ItemId : '']);
      }
    }
  }

  /**
   * ADDING THIS METHOD TO BE ABLE TO RID OFF FAVORITES
   * Returns a Computed tooltip which is tooltip property or defaults to text if no tooltip is defined.
   */
  tooltipAttribute(action: ScBizFxAction): string {
    return action.Description || action.DisplayName;
  }

  /**
   * ADDING THIS METHOD TO BE ABLE TO RID OFF FAVORITES
   * Helper method that contains logic for when to auto-promote actions.
   * Should auto-promote all actions if there is 3 or less actions.
   * @returns Returns true when there is maximum 3 actions in total.
   */
  shouldAutoPromoteActions(): boolean {
    return this.view && this.view.Actions.length < 4;
  }
}
