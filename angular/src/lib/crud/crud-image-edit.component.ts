import { ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';

import { CommonBreakpointObserver } from 'src/lib/components/common-breakpointobserver/common-breakpointobserver.service';
import { iCrudImageDto } from './crud.interface';
import { CrudImageState } from './crud-image-state.dto';
import { CrudEditService } from './crud-edit.service';
import { CrudEditComponent } from './crud-edit.component';


export abstract class CrudImageEditComponent<TReadDto extends iCrudImageDto, TCreateUpdateDto>
        extends CrudEditComponent<TReadDto, TCreateUpdateDto> {
    private imageType!: string | null;

    
    public constructor(snackBar: MatSnackBar,
                       route: ActivatedRoute,
                       location: Location,
                       dialog: MatDialog,               
                       changeDetectorRef: ChangeDetectorRef,
                       commonBreakpointObserver: CommonBreakpointObserver,
                       entityService: CrudEditService<TReadDto, TCreateUpdateDto>,
                       headerName: string) {
        super(snackBar,
              route,
              location,
              dialog,
              changeDetectorRef,
              commonBreakpointObserver,
              entityService,
              headerName);
    }

    
    public onImageChange(event: any): void {
        const reader = new FileReader();
        if (event.target.files && event.target.files.length > 0) {
            const eventFile = <File>event.target.files[0];
            reader.readAsDataURL(eventFile);
            reader.onload = () => {
                this.dto.imageState = CrudImageState.New;
                this.dto.imageFilename = eventFile.name;
                this.dto.imageContent = (<string>reader.result).split(',')[1];
                this.dto.imageUrl = null;
                this.imageType = eventFile.type;
                this.dirty = true;
            };
        }
    }


    public onImageClear(): void {
        this.dto.imageState = CrudImageState.Deleted;
        this.dto.imageFilename = null;
        this.dto.imageContent = null;
        this.dto.imageUrl = null;
        this.imageType = null;;
        this.dirty = true;
    }


    public imageSrc(): string | null {
        if (!this.dto) {
            return null;
        }
        
        if (this.dto.imageContent) {
            return 'data:' + this.imageType + ';base64,' + this.dto.imageContent;
        }

        if (this.dto.imageUrl) {
            return this.dto.imageUrl;
        }

        return null;
    }
}