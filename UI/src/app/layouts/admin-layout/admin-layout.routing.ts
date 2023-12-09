import { Routes } from '@angular/router';

import { CategoryComponent } from 'app/category/category.component';
import { ItemComponent } from 'app/item/item.component';

export const AdminLayoutRoutes: Routes = [
    { 
        path: 'category',
        component: CategoryComponent 
    },
    { 
        path: 'item',
        component: ItemComponent 
    },
    { 
        path: '',
        component: CategoryComponent 
    }
];
