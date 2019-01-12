import { Injectable } from "@angular/core";
import { Resolve, Router } from "@angular/router";
import { UserService } from "../_services/user.service";
import { AlertifyService } from "../_services/alertify.service";
import { AuthService } from "../_services/auth.service";
import { Observable, of } from "rxjs";
import { catchError } from 'rxjs/operators';
import { User } from "../_models/user";

@Injectable()

export class MemberEditResolver implements Resolve<User>
{
    constructor(private userService: UserService , private router: Router, private alterify: AlertifyService , private authService: AuthService)
    {}

    resolve(): Observable<User>
    {
        return this.userService.getUser(this.authService.decodedToken.nameid).pipe(
            catchError(error => {
                this.alterify.error('Problem retrieving your data');
                this.router.navigate(['/members']);
                return of(null);
            })
        )

    }
}