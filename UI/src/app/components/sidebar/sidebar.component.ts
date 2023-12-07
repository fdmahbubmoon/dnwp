import { Component, OnInit } from '@angular/core';
import { TokenVm } from 'app/models/tokenVm';
import { CookieService } from 'ngx-cookie-service';

declare const $: any;
declare interface RouteInfo {
    path: string;
    title: string;
    icon: string;
    class: string;
}
export const ROUTES: RouteInfo[] = [
    { path: '/admin/category', title: 'Category',  icon: 'dashboard', class: '' },
    { path: '/admin/item', title: 'Item',  icon: 'dashboard', class: '' }
];

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent implements OnInit {
  menuItems: any[];

  constructor(private cookieService: CookieService) { }

  ngOnInit() {
    let token = this.cookieService.get('auth_token');
    if(!token)
      return;
    let parsedToken = JSON.parse(token) as TokenVm;
    if(parsedToken.roles.includes('Admin')){
      this.menuItems = ROUTES.filter(menuItem => menuItem);
    }
    else{
      this.menuItems = ROUTES.filter(menuItem => menuItem && menuItem.title == 'Item');
    }
  }
  isMobileMenu() {
      if ($(window).width() > 991) {
          return false;
      }
      return true;
  };
}
