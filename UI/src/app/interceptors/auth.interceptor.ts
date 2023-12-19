import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { TokenVm } from 'app/models/tokenVm';
import { AuthService } from 'app/services/auth.service';
import { environment } from 'environments/environment';
import { CookieService } from 'ngx-cookie-service';
import { Observable, catchError, finalize, map, of, tap, throwError } from 'rxjs';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private API_URL= environment.apiBaseUrl;

  constructor(private authService: AuthService, private router: Router) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    
    req = req.clone({
      url: this.API_URL + req.url
    });
    
    let isAuthRequest = req.url == (this.API_URL + 'Auth');

    if(!isAuthRequest){
        let token = this.authService.getToken() ?? {token_type: '', access_token: ''};
        const authReq = req.clone({
          headers: req.headers.set('Authorization', token.token_type + ' ' + token.access_token)
        });

        return next.handle(authReq).pipe(
            map((event: HttpEvent<any>) => {
                return event;
            }),
            catchError(x=> this.handleAuthError(x))
        );
    }

    return next.handle(req).pipe(map((event: HttpEvent<any>) => {
        if (event instanceof HttpResponse) {
            if(isAuthRequest)
                this.authService.setToken(event.body);
        }
        return event;
    }));

  }

  private handleAuthError(err: HttpErrorResponse): Observable<any> {
    //handle your auth error or rethrow
    if (err.status === 401 || err.status === 403) {
        //navigate /delete cookies or whatever
        this.router.navigateByUrl(`/login`);
        // if you've caught / handled the error, you don't want to rethrow it unless you also want downstream consumers to have to handle it as well.
        return of(err.message); // or EMPTY may be appropriate here
    }
    return throwError(err);
  }

}