import type { NextPage } from 'next';
import { useDispatch } from "react-redux";

import { add, remove, AppDispatch, useProducts, ProductsState } from '@store/products';
import { ProductApiModel } from '@store/products-api';

const id = () => (Math.random() + 1).toString(36).substring(7);

const removeProduct = (dispatch, productId) => dispatch(remove(productId));

const renderProducts = (dispatch, products: ProductsState) => {
  const productsArr = Object.values(products);
  if (!productsArr) return null;

  return productsArr.map(p => {
    return (
      <div key={p.id}>
        {p.name}
        <button onClick={() => removeProduct(dispatch, p.id)}>Remove</button>
      </div>
    );
  });
};

const Products: NextPage = () => {
  const dispatch = useDispatch<AppDispatch>();
  const products = useProducts();

  return (
    <>
      <h2>Products:</h2>
      <br/>
      <button onClick={() => dispatch(add({ id: id(), name: id() } as ProductApiModel))}>
        Add Product
      </button>
      {renderProducts(dispatch, products)}
    </>
  );
};

export default Products;
