import type { NextPage } from 'next';
import { useDispatch } from "react-redux";

import { add, remove, AppDispatch, useProducts } from '@store/products';
import { ProductApiModel } from '@store/products-api';

// TODO: here for testing. REMOVE LATER!
const id = () => (Math.random() + 1).toString(36).substring(7);

const Products: NextPage = () => {
  const dispatch = useDispatch<AppDispatch>();

  const products = useProducts();
  const productsArr = Object.values(products);

  return (
    <div className="ui container">
      <h2>Products:</h2>
      <br/>
      <table className="ui compact celled definition table">
        <thead className="full-width">
        <tr>
          <th>Name</th>
          <th/>
        </tr>
        </thead>
        <tbody>
        {productsArr && productsArr.map(p =>
          (
            <tr key={p.id}>
              <td>
                {p.name}
              </td>
              <td>
                <button className="ui button red" onClick={() => dispatch(remove(p.id))}>Remove</button>
              </td>
            </tr>
          ))}
        </tbody>
        <tfoot className="full-width">
        <tr>
          <th/>
          <th>
            <div
              className="ui right floated small primary labeled icon button"
              onClick={() => dispatch(add({ id: id(), name: id() } as ProductApiModel))}>
              <i className="user icon" /> Add Product
            </div>
          </th>
        </tr>
        </tfoot>
      </table>
    </div>
  );
};

export default Products;
