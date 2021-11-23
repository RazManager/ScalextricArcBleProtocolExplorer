import { CrudImageState } from './crud-image-state.dto';


export interface iIdDto {
    id: string;
}


export interface iCrudDto extends iIdDto {
    createdAt: Date | null;
    createdUserName: string | null;
    updatedAt: Date | null;
    updatedUserName: string | null;
}


export interface iCrudImageDto extends iCrudDto {
    imageState: CrudImageState;
    imageContent: string | null;
    imageFilename: string | null;
    imageUrl: string | null;
}
