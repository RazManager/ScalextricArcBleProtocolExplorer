import { Component, OnInit, NgZone } from '@angular/core';
import { ActivatedRoute, Data } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ScrollDispatcher } from '@angular/cdk/overlay';

import { iIdDto } from './crud.interface';
import { CrudListBaseComponent } from './crud-list-base.component';
import { CrudListMasterService } from './crud-list-master.service';
import { ApiErrorResponse } from '../common/common-base.service';


@Component({ template: '' })
export abstract class CrudListMasterComponent<TListDto, TSelectDto extends iIdDto>
        extends CrudListBaseComponent<TListDto>
        implements OnInit {
    constructor(protected readonly route: ActivatedRoute,
                snackBar: MatSnackBar,
                scrollDispatcher: ScrollDispatcher,
                ngZone: NgZone,
                protected readonly entityService: CrudListMasterService<TListDto, TSelectDto>) {
        super(snackBar,
              scrollDispatcher,
              ngZone,
              entityService);
    }


    public ngOnInit(): void {
        this.route.data.subscribe({
            next: (data: Data) => {
                this.onEntitiesRetrieved(<TListDto[]>data.result)
            }
        });
    }


    protected getEntities(): void {
        this.entityService.readEntities(this.limitFirst, this.limitFollowing, this.items.length)
                          .subscribe({
                                next: (res: TListDto[]) => this.onEntitiesRetrieved(res),
                                error: (err: ApiErrorResponse) => this.onError(err)
                          });
    }
}
