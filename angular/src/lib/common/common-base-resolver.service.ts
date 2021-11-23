import { MatSnackBar } from '@angular/material/snack-bar';

import { ApiErrorResponse } from './common-base.service';
import { Observable, EMPTY } from 'rxjs';


export abstract class CommonBaseResolver {
    constructor(private readonly snackBar: MatSnackBar) { }


    protected onError(err: ApiErrorResponse): Observable<never> {
        this.snackBar.open(err.title, undefined, {
            duration: 5000
        });

        return EMPTY;
    }
}
