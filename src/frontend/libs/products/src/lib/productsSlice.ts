import { ProductApiModel } from '@store/products-api';

import { createSlice, PayloadAction } from '@reduxjs/toolkit'

export interface ProductsState {
  [Key: string]: ProductApiModel
}

const initialState: ProductsState = {};

const productsSlice = createSlice({
  name: 'products',
  initialState,
  reducers: {
    add(state, action: PayloadAction<ProductApiModel>) {
      const product = action.payload;
      if (!product.id) return;

      state[product.id] = product;
    },
    remove(state, action: PayloadAction<string>) {
      delete state[action.payload];
    }
  },
})

export const { add, remove } = productsSlice.actions
export default productsSlice.reducer
