import { NgModule } from '@angular/core';
import { RouterModule, Routes, Data, Params } from '@angular/router';

import { ScBizFxContextResolver, ScAuthGuard } from '@sitecore/bizfx';

import { AppComponent } from './app.component';
import { ScBizFxViewComponent, ScBizFxActionComponent } from './components';

const routes: Routes = [
  { path: '', redirectTo: '/', pathMatch: 'full' },
  {
    path: 'entityView/:viewName',
    component: ScBizFxViewComponent,
    canActivate: [ScAuthGuard],
    resolve: { config: ScBizFxContextResolver },
    data: { pageType: 'dashboard' }
  },
  {
    path: 'entityView/:viewName/:entityId',
    component: ScBizFxViewComponent,
    canActivate: [ScAuthGuard],
    resolve: { config: ScBizFxContextResolver },
    data: { pageType: 'task' }
  },
  {
    path: 'entityView/:viewName/:entityId/:itemId',
    component: ScBizFxViewComponent,
    canActivate: [ScAuthGuard],
    resolve: { config: ScBizFxContextResolver },
    data: { pageType: 'task' }
  },

  {
    path: 'action/:viewName/:entityId/:actionName',
    component: ScBizFxActionComponent,
    canActivate: [ScAuthGuard],
    resolve: { config: ScBizFxContextResolver },
    data: { pageType: 'task' }
  },
  {
    path: 'action/:viewName/:entityId/:actionName/:itemId',
    component: ScBizFxActionComponent,
    canActivate: [ScAuthGuard],
    resolve: { config: ScBizFxContextResolver },
    data: { pageType: 'task' }
  },
  {
    path: 'action/:viewName/:actionName',
    component: ScBizFxActionComponent,
    canActivate: [ScAuthGuard],
    resolve: { config: ScBizFxContextResolver },
    data: { pageType: 'task' }
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
  providers: [ScBizFxContextResolver]
})

export class AppRoutingModule { }
