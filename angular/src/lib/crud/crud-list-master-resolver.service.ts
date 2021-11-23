import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Observable } from 'rxjs';
import { map, catchError } from 'rxjs/operators';

import { ApiErrorResponse } from '../common/common-base.service';
import { CommonBaseResolver } from '../common/common-base-resolver.service';
import { CrudListMasterService } from './crud-list-master.service';


export abstract class CrudListMasterResolver<TListDto, TSelectDto>
        extends CommonBaseResolver
        implements Resolve<TListDto[]> {

    public constructor(snackBar: MatSnackBar,
                       private readonly entityService: CrudListMasterService<TListDto, TSelectDto>) {
        super(snackBar);
    }


    public resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<TListDto[]> {
        return this.entityService.readEntities(30, 5, 0)
                          .pipe(catchError((err: ApiErrorResponse) => this.onError(err)));
    }
}
