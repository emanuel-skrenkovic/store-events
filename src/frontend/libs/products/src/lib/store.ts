import { useSelector } from 'react-redux';
import { configureStore } from '@reduxjs/toolkit'
import productsReducer from './productsSlice';

export const productsStore = configureStore({
  reducer: { products: productsReducer },
})

export type RootState = ReturnType<typeof productsStore.getState>
export type AppDispatch = typeof productsStore.dispatch

export const useProducts = () => useSelector((state: RootState) => state.products);
