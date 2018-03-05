import { SitecoreBizfxPage } from './app.po';

describe('sitecore-bizfx App', () => {
  let page: SitecoreBizfxPage;

  beforeEach(() => {
    page = new SitecoreBizfxPage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
