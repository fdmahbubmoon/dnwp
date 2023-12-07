import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { TokenVm } from "app/models/tokenVm";
import { CookieService } from "ngx-cookie-service";
import { Observable } from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private cookieService: CookieService) {
  }

  getToken(): TokenVm{
    let token = this.cookieService.get('auth_token');
    if(!token)
        return null;
    return JSON.parse(token);
  }

  setToken(token: TokenVm){
    this.cookieService.set('auth_token', JSON.stringify(token));
  }

  resetToken(){
    this.cookieService.delete('auth_token');
  }
}