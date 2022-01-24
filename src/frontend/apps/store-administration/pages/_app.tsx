import { AppProps } from 'next/app';
import Head from 'next/head';
import { Provider } from 'react-redux';

import { productsStore } from '@store/products';

import './styles.css';

function CustomApp({ Component, pageProps }: AppProps) {
  return (
    <div className="container">
      <Head>
        <title>Welcome to store-administration!</title>
      </Head>
      <main className="app">
        <Provider store={productsStore}>
          <Component {...pageProps} />
        </Provider>
      </main>
    </div>
  );
}

export default CustomApp;
