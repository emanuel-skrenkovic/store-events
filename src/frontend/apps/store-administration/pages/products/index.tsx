import type { NextPage } from 'next';
import { useDispatch } from "react-redux";

import { add, remove, AppDispatch, useProducts } from '@store/products';
import { ProductApiModel } from '@store/products-api';
import type { TableHeader } from '@store/shared';
import { ActionTable } from '@store/shared';

// TODO: here for testing. REMOVE LATER!
const id = () => (Math.random() + 1).toString(36).substring(7);

const tableHeaders: TableHeader[] = [
  { propertyName: 'id',   displayName: 'Id',   sortable: false, isKey: true },
  { propertyName: 'name', displayName: 'Name', sortable: true },
];

const Index: NextPage = () => {
  const dispatch = useDispatch<AppDispatch>();

  const products = useProducts();
  const productsArr = Object.values(products);

  return (
    <>
      <ActionTable
        headers={tableHeaders}
        data={productsArr}

        onItemDelete={(product: ProductApiModel) => dispatch(remove(product.id))}
        onItemEdit={() => { console.log('on item edit')}}

        pageNumber={1}
        pageSize={10}
        totalPages={1}

        onNextPage={() => { console.log('on next page')}}
        onPreviousPage={() => { console.log('on previous page')}}
        onChangePageSize={() => { console.log('on change page size')}}
      />
      <button onClick={() => dispatch(add({ id: id(), name: id() } as ProductApiModel))}>
        Add Product
      </button>
    </>
  );
};

export default Index;
