import { Component, OnDestroy } from '@angular/core';
import { MatSnackBar, MatSnackBarRef, SimpleSnackBar } from '@angular/material/snack-bar';

import { ApiErrorResponse } from './common-base.service';


@Component({ template: '' })
export abstract class CommonBaseComponent
        implements OnDestroy {
    private snackBarRef: MatSnackBarRef<SimpleSnackBar> | null = null;


    constructor(private readonly snackBar: MatSnackBar) { }


    public ngOnDestroy(): void {
        if (this.snackBarRef !== null) {
            this.snackBarRef.dismiss();
        }
    }


    protected onError(err: ApiErrorResponse): void {
        this.snackBarRef = this.snackBar.open(err.title, undefined, {
            duration: 5000
        });
    }


    protected snackBarOpen(message: string) {
        this.snackBarRef = this.snackBar.open(message, undefined, {
            duration: 5000
        });
    }


    public localeString(date: Date | null): string {
        if (!date) {
            return "";
        }
        return (new Date(date)).toLocaleString(undefined, {
            year: "numeric",
            month: "2-digit",
            day: "2-digit",
            hour: "2-digit",
            minute: "2-digit"
        });
    }    
}
