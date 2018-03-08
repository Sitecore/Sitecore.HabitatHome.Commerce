import { Component, OnInit, Inject } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';

import { TranslateService } from '@ngx-translate/core';

import { ScBizFxViewsService, ScBizFxAuthService, ScBizFxContextService } from '@sitecore/bizfx';

@Component({
  // tslint:disable-next-line:component-selector
  selector: 'app-root',
  styles: [`
  i:hover {
    color: gray;
  }

  .homeicon {
    color: white
  }
`],
  templateUrl: './app.component.html'
})

export class AppComponent implements OnInit {
  appName = '';
  pageTitle = 'Sitecore Experience Commerce';
  routes = [];
  isNavigationShown = true;
  isTaskPage = false;
  isHomePage = true;
  message: string;

  constructor(
    private viewsService: ScBizFxViewsService,
    private authService: ScBizFxAuthService,
    private router: Router,
    private bizFxContext: ScBizFxContextService,
    protected translate: TranslateService) {
  }

  ngOnInit() {
    this.createRoutes();

    this.viewsService.latestView$.subscribe(view => {
      this.pageTitle = view.DisplayName;
      this.createRoutes({
        Text: view.DisplayName
      });
    });

    this.viewsService.pageType$.subscribe(type => {
      Promise.resolve(null).then(() => {
        this.isTaskPage = type === 'task';
        this.isHomePage = type === 'home';
      });
    });

    this.router.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        this.isNavigationShown = false;

        if (event.url === '/') {
          this.pageTitle = 'Sitecore Experience Commerce';
          this.viewsService.announcePageType('home');
          this.createRoutes();
        }
      }
    });

    this.viewsService.errorAnnounced$.subscribe(
      error => {
        this.message = error;
        setTimeout(() => this.message = null, 2000);
      });

    this.bizFxContext.config$.subscribe(config => {
      this.translate.setDefaultLang(config.Language);
      this.translate.use(this.bizFxContext.language);
     });
    this.bizFxContext.language$.subscribe(language => this.translate.use(language));
  }

  createRoutes(route?) {
    const standardRoute = {
      Link: '/',
      Text: 'COMMERCE'
    };

    const routes = [standardRoute];

    if (route) {
      routes.push(route);
    }

    this.routes = routes;
  }
}
