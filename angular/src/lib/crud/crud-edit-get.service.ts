import { Router, ActivatedRouteSnapshot } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Observable } from 'rxjs';
import { map, catchError } from 'rxjs/operators';

import { ApiErrorResponse } from '../common/common-base.service';
import { CommonBaseResolver } from '../common/common-base-resolver.service';
import { iCrudDto } from './crud.interface';
import { CrudEditService } from './crud-edit.service';


export abstract class CrudEditRead<TReadDto extends iCrudDto, TCreateUpdateDto>
        extends CommonBaseResolver {

    public constructor(snackBar: MatSnackBar,
                       private readonly router: Router,                       
                       private readonly entityService: CrudEditService<TReadDto, TCreateUpdateDto>) {
        super(snackBar);
    }


    public get(route: ActivatedRouteSnapshot): Observable<[TReadDto, string | null, boolean]> {
        let id: string;
        let add = false;
        if (this.entityService.addedId) {
            id = this.entityService.addedId;
            const routeConfigPath = route.routeConfig!.path!;
            this.router.navigate(['/' + routeConfigPath.substr(0,  routeConfigPath.length - 4), id], { replaceUrl: true });
        } else {
            id = <string>route.params['id'];
            add = id.startsWith('add');
        }

        if (add) {
            return this.entityService.readDefaultEntity(route.queryParams)
            .pipe(
                map(res => this.onEntityRetrievedDefault(res)),
                catchError((err: ApiErrorResponse) => this.onError(err))
            );
        } else {
            return this.entityService.readEntity(id)
                .pipe(
                    map((res: [TReadDto, string]) => this.onEntityRetrievedRead(res)),
                    catchError((err: ApiErrorResponse) => this.onError(err))
                );
        }
    }


    protected onEntityRetrievedRead(res: [TReadDto, string]): [TReadDto, string, boolean] {
        return [res[0], res[1], false];
    }


    protected onEntityRetrievedDefault(res: TReadDto): [TReadDto, null, boolean] {
        return [<TReadDto><any>res, null, true];
    }


    // protected onEntityRetrievedCreateUpdate(res: TCreateUpdateDto): [TReadDto, string, boolean] {
    //     return [<TReadDto><any>res, undefined, true];
    // }
}
