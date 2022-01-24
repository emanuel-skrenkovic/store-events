import React, { useState, ChangeEvent, KeyboardEvent } from 'react';

export interface TableHeader {
  isKey?: boolean | false;
  propertyName: string;
  displayName: string;
  sortable: boolean;
}

export type TableItem = any;

export interface TableProps {
  onChangeSort?: undefined | ((sortOrder: string | undefined, sortBy: string | undefined) => void);

  headers: TableHeader[];
  data: TableItem[]; // TODO

  onItemEdit: (item: TableItem) => void;
  onItemDelete: (item: TableItem) => void;

  pageNumber: number;
  pageSize: number;
  totalPages: number;
  onNextPage: (nextPage: number) => void;
  onPreviousPage: (previousPage: number) => void;
  onChangePageSize: (newPageSize: number) => void;
}

// TODO: think about splitting into TableHeader, TableBody and TableFooter components which are passed as props children.
export const ActionTable: React.FC<TableProps> = (props: TableProps) => {
  // props data
  const { onChangeSort } = props;
  const { headers, data } = props;
  const { onChangePageSize} = props;
  const { onItemEdit, onItemDelete } = props;
  const { onNextPage, onPreviousPage } = props;
  const { pageNumber, pageSize, totalPages } = props;

  // local state
  const [sort, setSort] = useState<{ sortOrder: string, sortBy: string | undefined }>({ sortOrder: '', sortBy: undefined });
  const [inputPageSize, setInputPageSize] = useState<number | undefined>(pageSize);

  // event handlers
  // Loop through sorting in order of : None, Descending, Ascending, None. TODO: Consider modulus.
  const onClickSortableHeader = (sortByEvent: string) => {
    if (!onChangeSort) return;

    let newSort: { sortOrder: string, sortBy: string | undefined };

    switch (sort.sortOrder) {
      case 'ascending':
        newSort = { sortOrder: '', sortBy: undefined };
        break;

      case 'descending':
        newSort = { sortOrder: 'ascending', sortBy: sortByEvent };
        break;

      default:
        newSort = { sortOrder: 'descending', sortBy: sortByEvent };
    }

    const { sortOrder, sortBy } = newSort;

    onChangeSort(sortOrder, sortBy);
    setSort(newSort);
  };

  const onClickNextPage = () => {
    const nextPageNumber = pageNumber + 1;
    if (nextPageNumber > totalPages) return;

    onNextPage(nextPageNumber);
  };

  const onClickPreviousPage = () => {
    const previousPageNumber = pageNumber - 1;
    if (previousPageNumber < 1) return;

    onPreviousPage(previousPageNumber);
  };

  const onClickItemEdit = (item: TableItem) => {
    onItemEdit(item);
  };

  const onClickItemDelete = (item: TableItem) => {
    onItemDelete(item);
  };

  const onChangePageSizeInput = (e: ChangeEvent<HTMLInputElement>) => {
    // onChangePageSize(Number(e.currentTarget.value));
    const value = e.currentTarget.value;

    if (!value) setInputPageSize(undefined);
    else setInputPageSize(Number(value));
  };

  const onClickChangePageSizeInput = () => {
    if (!inputPageSize) return;
    onChangePageSize(inputPageSize);
  };

  const onPageSizeKeyDown = (e: KeyboardEvent<HTMLInputElement>) => {
    if (e.key !== 'Enter') return;
    return onClickChangePageSizeInput();
  };

  // renders
  const renderItems = () => {
    if (!data) return null;

    return data.map((item, c) => {
      // Find all the headers that are applicable to the object.
      // TODO: should probably just change to break if the property does not exist.
      const applicableHeaders =  headers.filter(h => Object.keys(item).includes(h.propertyName)).map(h => h.propertyName);

      // Row key property as marked in the headers array.
      const rowKeyPropertyName = headers.find(h => h.isKey);
      // if no table key property is found, simply add counter as row key.
      const key = rowKeyPropertyName ? item[rowKeyPropertyName.propertyName] : c;

      return (
        <tr key={key}>
          {applicableHeaders.map(h => (<td key={h}>{item[h]}</td>))}
          <td key="buttons">
            <button onClick={() => onClickItemDelete(item)}>
              Delete
            </button>
            <button onClick={() => onClickItemEdit(item)}>
              Edit
            </button>
          </td>
        </tr>
      );
    });
  };

  const renderHeaders = () => {
    if (!headers) return null;

    return headers.map(h => (
      <th key={h.propertyName}
          className={h.sortable && sort.sortBy === h.propertyName ? `sorted ${sort.sortOrder}` : ''}
          onClick={h.sortable ? () => onClickSortableHeader(h.propertyName) : undefined}>
        {h.displayName}
      </th>
    ));
  };

  return (
    <table>
      <thead>
      <tr>
        {renderHeaders()}
        <th key="empty"/>
      </tr>
      </thead>
      <tbody>
      {renderItems()}
      </tbody>
      <tfoot>
      <tr>
        <th key="paging">
          <button onClick={onClickPreviousPage}>Previous</button>
          <button onClick={onClickNextPage}>Next</button>
          <label>Page&nbsp;{pageNumber}/{totalPages}</label>
        </th>
        {headers && Array.from(Array(headers.length - 1).keys()).map((_, i) => <th key={i} />) }
        <th key="pageSize">
          <label htmlFor="pageSize">Page Size</label>
          <input name="pageSize" type="number" value={inputPageSize} onChange={onChangePageSizeInput} onKeyDown={onPageSizeKeyDown} />
          <button className={`ui ${!inputPageSize ? 'disabled' : ''} icon button`} onClick={onClickChangePageSizeInput}>
            Apply
          </button>
        </th>
      </tr>
      </tfoot>
    </table>
  );
};
