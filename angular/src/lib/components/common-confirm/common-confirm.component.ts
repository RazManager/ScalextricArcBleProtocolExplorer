import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';


@Component({
    templateUrl: './common-confirm.component.html',
})
export class CommonConfirmComponent {
    constructor(private readonly dialogRef: MatDialogRef<CommonConfirmComponent>,
                @Inject(MAT_DIALOG_DATA) public readonly data: string) { }


    public confirm(): void {
        this.dialogRef.close(true);
    }
}
