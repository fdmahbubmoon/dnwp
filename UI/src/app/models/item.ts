

export interface Item {
    id?: number;
    isEditEnabled?: boolean;
    itemName: string;
    itemUnit: string;
    itemQuantity?: number | null;
    categoryId?: number | null;
    categoryName?: string;
  }