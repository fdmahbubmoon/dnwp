import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { TokenVm } from 'app/models/tokenVm';
import { CookieService } from 'ngx-cookie-service';
import { Observable, catchError, finalize, map, of, tap, throwError } from 'rxjs';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private cookieService: CookieService, private router: Router) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    let isAuthRequest = req.url == 'https://localhost:7024/api/Auth';
    
    if(!isAuthRequest){
        console.log('Not Auth req');
        let token = this.getToken();

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
                this.setToken(event.body);
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

  getToken(): TokenVm{
    let token = this.cookieService.get('auth_token');
    return JSON.parse(token);
  }
  setToken(token: TokenVm){
    this.cookieService.set('auth_token', JSON.stringify(token));
  }
}