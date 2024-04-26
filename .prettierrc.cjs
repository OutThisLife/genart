module.exports = {
  plugins: [
    require.resolve('prettier-plugin-astro'),
    require.resolve('prettier-plugin-css-order')
  ],
  arrowParens: 'avoid',
  bracketSpacing: true,
  endOfLine: 'auto',
  jsxBracketSameLine: true,
  parser: 'typescript',
  printWidth: 80,
  semi: false,
  singleQuote: true,
  tabWidth: 2,
  trailingComma: 'none',
  useTabs: false,
  overrides: [
    {
      files: '*.astro',
      options: {
        parser: 'astro'
      }
    }
  ]
}
